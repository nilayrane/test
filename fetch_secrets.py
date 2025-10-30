import json
import os

# Suppose you fetch from some secure API or vault
secrets = {
    "MY_SECRET": "super_secret_token_value",
    "API_KEY": "abcd-1234"
}

# Azure DevOps requires this syntax to set variables at runtime
for key, value in secrets.items():
    # Mask secret from logs
    print(f"##vso[task.setvariable variable={key};issecret=true]{value}")
