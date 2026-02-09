#!/bin/bash
# ─────────────────────────────────────────────────────────
# Keycloak Realm Provisioning — SwiftApp Postal
# Creates the swiftapp-postal realm, client, roles, and
# test users. Idempotent — safe to re-run.
# ─────────────────────────────────────────────────────────

KCADM=/opt/keycloak/bin/kcadm.sh
REALM=swiftapp-postal
CLIENT_ID=swiftapp-postal-client
CLIENT_SECRET="${KEYCLOAK_CLIENT_SECRET:-change-me-in-production}"

echo "Waiting for Keycloak to be ready..."
until $KCADM config credentials \
    --server http://localhost:8080 \
    --realm master \
    --user "$KEYCLOAK_ADMIN" \
    --password "$KEYCLOAK_ADMIN_PASSWORD" 2>/dev/null; do
    sleep 3
done
echo "Keycloak is ready."

# Skip if realm already exists (handles both fresh starts and restarts)
if $KCADM get realms/$REALM --fields realm 2>/dev/null | grep -q "\"$REALM\""; then
    echo "Realm '$REALM' already provisioned in Keycloak. Skipping."
    exit 0
fi

echo "Provisioning realm '$REALM'..."

# ── Realm ──────────────────────────────────────────────
echo "Creating realm '$REALM'..."
$KCADM create realms \
    -s realm=$REALM \
    -s enabled=true \
    -s displayName="SwiftApp Postal"

# ── Realm Roles ────────────────────────────────────────
echo "Ensuring realm roles..."
for ROLE in ADMIN BRANCH_MANAGER EMPLOYEE; do
    if ! $KCADM get roles -r $REALM --fields name 2>/dev/null | grep -q "\"$ROLE\""; then
        $KCADM create roles -r $REALM -s name="$ROLE"
        echo "  Created role: $ROLE"
    fi
done

# ── Client ─────────────────────────────────────────────
echo "Ensuring client '$CLIENT_ID'..."
CLIENT_UUID=$($KCADM get clients -r $REALM -q clientId=$CLIENT_ID --fields id 2>/dev/null \
    | grep '"id"' | head -1 | sed 's/.*: "\(.*\)".*/\1/')

if [ -z "$CLIENT_UUID" ]; then
    $KCADM create clients -r $REALM \
        -s clientId=$CLIENT_ID \
        -s enabled=true \
        -s publicClient=false \
        -s "secret=$CLIENT_SECRET" \
        -s 'redirectUris=["http://localhost:5100/*","http://localhost:5101/*","http://localhost:8080/*","http://127.0.0.1:5100/*","http://127.0.0.1:5101/*","http://0.0.0.0:5100/*","http://0.0.0.0:5101/*"]' \
        -s 'webOrigins=["http://localhost:5100","http://localhost:5101","http://127.0.0.1:5100","http://127.0.0.1:5101","http://0.0.0.0:5100","http://0.0.0.0:5101"]' \
        -s directAccessGrantsEnabled=true \
        -s standardFlowEnabled=true
    CLIENT_UUID=$($KCADM get clients -r $REALM -q clientId=$CLIENT_ID --fields id 2>/dev/null \
        | grep '"id"' | head -1 | sed 's/.*: "\(.*\)".*/\1/')
    echo "  Created client: $CLIENT_ID"
else
    echo "  Client exists: $CLIENT_ID"
fi

# ── Protocol Mapper: realm roles → tokens ──────────────
echo "Ensuring realm roles are mapped to ID/access tokens..."
ROLES_SCOPE_ID=$($KCADM get client-scopes -r $REALM --fields id,name 2>/dev/null | python3 -c "
import sys,json
data = json.load(sys.stdin)
for s in data:
    if s.get('name') == 'roles': print(s['id']); break
" 2>/dev/null)

if [ -n "$ROLES_SCOPE_ID" ]; then
    REALM_ROLES_MAPPER_ID=$($KCADM get client-scopes/$ROLES_SCOPE_ID/protocol-mappers/models \
        -r $REALM --fields id,name 2>/dev/null | python3 -c "
import sys,json
data = json.load(sys.stdin)
for m in data:
    if m.get('name') == 'realm roles': print(m['id']); break
" 2>/dev/null)

    if [ -n "$REALM_ROLES_MAPPER_ID" ]; then
        $KCADM update client-scopes/$ROLES_SCOPE_ID/protocol-mappers/models/$REALM_ROLES_MAPPER_ID \
            -r $REALM \
            -b '{
                "name": "realm roles",
                "protocol": "openid-connect",
                "protocolMapper": "oidc-usermodel-realm-role-mapper",
                "consentRequired": false,
                "config": {
                    "multivalued": "true",
                    "id.token.claim": "true",
                    "access.token.claim": "true",
                    "userinfo.token.claim": "true",
                    "claim.name": "realm_access.roles",
                    "jsonType.label": "String"
                }
            }' 2>/dev/null && echo "  Updated realm roles mapper (included in ID + access token)"
    fi
fi

# ── Test Users ─────────────────────────────────────────
create_user() {
    local USERNAME=$1 PASSWORD=$2 ROLE=$3 FIRST=$4 LAST=$5 EMAIL=$6

    if ! $KCADM get users -r $REALM -q username=$USERNAME --fields username 2>/dev/null | grep -q "$USERNAME"; then
        $KCADM create users -r $REALM \
            -s username="$USERNAME" \
            -s enabled=true \
            -s emailVerified=true \
            -s "firstName=$FIRST" \
            -s "lastName=$LAST" \
            -s "email=$EMAIL"
        $KCADM set-password -r $REALM --username "$USERNAME" --new-password "$PASSWORD"
        echo "  Created user: $USERNAME"
    fi
    $KCADM add-roles -r $REALM --uusername "$USERNAME" --rolename "$ROLE" 2>/dev/null || true
    echo "  $USERNAME → $ROLE"
}

echo "Ensuring test users..."
create_user "admin-postal"    "pass123" "ADMIN"          "Hans"   "Müller"  "admin@swiftapp.ch"
create_user "manager1-postal" "pass123" "BRANCH_MANAGER" "Marie"  "Dupont"  "marie.dupont@swiftapp.ch"
create_user "manager2-postal" "pass123" "BRANCH_MANAGER" "Luca"   "Rossi"   "luca.rossi@swiftapp.ch"
create_user "manager3-postal" "pass123" "BRANCH_MANAGER" "Sophie" "Meier"   "sophie.meier@swiftapp.ch"
create_user "emp1-postal"     "pass123" "EMPLOYEE"       "Thomas" "Keller"  "thomas.keller@swiftapp.ch"
create_user "emp2-postal"     "pass123" "EMPLOYEE"       "Anna"   "Fischer" "anna.fischer@swiftapp.ch"
create_user "emp3-postal"     "pass123" "EMPLOYEE"       "Marco"  "Bianchi" "marco.bianchi@swiftapp.ch"
create_user "emp4-postal"     "pass123" "EMPLOYEE"       "Elena"  "Weber"   "elena.weber@swiftapp.ch"
create_user "emp5-postal"     "pass123" "EMPLOYEE"       "David"  "Brunner" "david.brunner@swiftapp.ch"
create_user "emp6-postal"     "pass123" "EMPLOYEE"       "Laura"  "Schmid"  "laura.schmid@swiftapp.ch"

echo ""
echo "=================================================="
echo "  Keycloak provisioning complete!"
echo "  Realm:   $REALM"
echo "  Client:  $CLIENT_ID"
echo "  Roles:   ADMIN, BRANCH_MANAGER, EMPLOYEE"
echo "  Users:   10 (1 admin, 3 managers, 6 employees)"
echo "=================================================="
