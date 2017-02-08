##How to build your own Encompass SDK Nuget package

###Step 1: Obtain the Encompass dlls from a legitimate source.  
We asked Ellie Mae for a zip file of everything we needed.
###Step 2: Copy them into a the folder EncompassFilesGoHere.

###Step 3: Run nuget
    `nuget pack EncompassSDK.Complete.nuspec`
You should now be the proud owner of a nuget package called EncompassSDK.Complete.17.1.0.0.nupkg