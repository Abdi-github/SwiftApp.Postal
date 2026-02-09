#!/bin/bash
# ─────────────────────────────────────────────────────────
# Keycloak Entrypoint for SwiftApp Postal
#   1. Generates a self-signed TLS keystore (dev mode)
#   2. Starts Keycloak in dev mode
#   3. Runs realm provisioning in the background
# ─────────────────────────────────────────────────────────
set -e

KEYSTORE=/opt/keycloak/conf/server.keystore
# Adjust keytool path if Java version differs
KEYTOOL=$(find /usr/lib/jvm -name "keytool" 2>/dev/null | head -1)
KEYTOOL=${KEYTOOL:-/usr/lib/jvm/java-21-openjdk/bin/keytool}

# Generate a self-signed keystore only if it doesn't exist
if [ ! -f "$KEYSTORE" ]; then
    echo "Generating self-signed TLS keystore for localhost..."
    $KEYTOOL -genkeypair \
        -alias server \
        -keyalg RSA \
        -keysize 2048 \
        -validity 365 \
        -dname "CN=localhost" \
        -keystore "$KEYSTORE" \
        -storepass password \
        -keypass password \
        -storetype PKCS12 2>/dev/null || true
    echo "Keystore generated at $KEYSTORE"
fi

# Run realm provisioning in the background
if [ -f /opt/keycloak/scripts/provision.sh ]; then
    echo "Starting background provisioning..."
    bash /opt/keycloak/scripts/provision.sh &
fi

# Start Keycloak in dev mode (HTTP only for simplicity in dev)
exec /opt/keycloak/bin/kc.sh start-dev
