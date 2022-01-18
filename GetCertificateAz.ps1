# get env account key
$accountKey = $env:ACCOUNTKEY
# get env account name
$accountName = $env:ACCOUNTNAME
# get env path certificate pfx
$pathCertAz = $env:PATHCERTAZ
# get env share name
$shareName = $env:SHARENAME
# get env name for project
$nameProject = $env:NAMEPROJECT 
# get env destination for file certificate
$publishDir = "./$env:PUBLISHDIR/$nameProject/"

# create folder to publish project
mkdir $publishDir

# az storage file certificate download
az storage file download --account-key $accountKey --account-name $accountName --path $pathCertAz --share-name $shareName --no-progress --dest $publishDir

pwd

ls $destCert