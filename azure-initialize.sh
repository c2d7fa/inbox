az group create \
  --name 'Inbox' \
  --location westeurope

az group update \
  --name 'Inbox' \
  --tags 'type=development'

az storage account create \
  --name 'johvinbox' \
  --resource-group 'Inbox' \
  --location westeurope

az functionapp create \
  --name 'johvinbox' \
  --resource-group 'Inbox' \
  --storage-account 'johvinbox' \
  --consumption-plan-location westeurope \
  --functions-version 3 \
  --disable-app-insights

