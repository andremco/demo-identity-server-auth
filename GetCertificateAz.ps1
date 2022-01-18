# get env account key
$accountKey = $env:ACCOUNTKEY
# get env account name
$accountName = $env:ACCOUNTNAME
# get env path certificate pfx
$pathCertAz = $env:PATHCERTAZ
# get env share name
$shareName = $env:SHARENAME
# get env destination for file certificate
$destCert = $env:DESTCERT

mkdir $destCert

# az storage file download
az storage file download --account-key $accountKey --account-name $accountName --path $pathCertAz --share-name $shareName --no-progress --dest $destCert

pwd

ls $destCert