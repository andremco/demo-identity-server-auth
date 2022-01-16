# get env account key
$accountKey = $env:ACCOUNTKEY
# get env account name
$accountName = $env:ACCOUNTNAME
# get env path certificate pfx
$pathCertsAz = $env:PATHCERTSAZ
# get env share name
$shareName = $env:SHARENAME

# az storage file download
az storage file download --account-key $accountKey --account-name $accountName --path $path --share-name $shareName --no-progress

ls