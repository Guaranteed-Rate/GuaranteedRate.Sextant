
$environments = ("dev", "stage", "prod")
 
git checkout master

foreach($environment in $environments){
	C:\git\GuaranteedRate.Sextant\GuaranteedRate.Sextant.CustomFieldComparer\bin\Debug\GuaranteedRate.Sextant.CustomFieldComparer.exe -j \\Mac\Home\Documents\encompass-config\$environment.json
}

write-host "adding some insane number of files."
git add .
write-host "committing some insane number of files."
git commit -m  ("Encompass state as of {0}" -f ((Get-Date -format s)))
write-host "pushing...."
git push