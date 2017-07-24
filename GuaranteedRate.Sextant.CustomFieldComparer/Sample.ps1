
$environments = ("dev", "stage", "prod")
 
git checkout master

foreach($environment in $environments){
	bin\Debug\GuaranteedRate.Sextant.CustomFieldComparer.exe -j encompass-config\$environment.json
}

write-host "adding some insane number of files."
git add .
write-host "committing some insane number of files."
git commit -m  ("Encompass state as of {0}" -f ((Get-Date -format s)))
write-host "pushing...."
git push