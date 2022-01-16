# get env account key
$accountKey = $env:ACCOUNTKEY
# get env account name
$accountName = $env:ACCOUNTNAME
# get env path certificate pfx
$path = "certs/ecdsaCert.pfx"
# get env share name
$shareName = "certs"

# az storage file download
az storage file download --account-key $accountKey --account-name $accountName --path $path --share-name $shareName --no-progress

ls