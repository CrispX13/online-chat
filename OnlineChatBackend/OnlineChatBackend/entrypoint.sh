#!/bin/sh
set -e

echo "Waiting for PostgreSQL at $PGHOST:$PGPORT..."
until nc -z "$PGHOST" "$PGPORT"; do
  sleep 1
done

echo "PostgreSQL is up, starting app..."
exec dotnet OnlineChatBackend.dll
