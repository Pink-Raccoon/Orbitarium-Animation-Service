$AnimationServiceFolder = "C:\Users\Tim\BA\Orbitarium-Animation-Service\Animation_Service\WebApplication1\Data\source\corona_data"
$AnimationFolder = "C:\Users\Tim\BA\Orbitarium-Animations\animation\corona_spread\data"

#Remove existing CSV
Remove-Item *.csv

#Get new Corona data
#GitHub repository: https://github.com/imdevskp/covid_19_jhu_data_web_scrap_and_cleaning
python data_cleaning.py

#Sort USA data
Rename-Item .\time_series_covid19_confirmed_US.csv -NewName temp.csv
Import-Csv .\temp.csv | Sort-Object {[int]$_.UID} | Export-Csv -Path .\time_series_covid19_confirmed_US.csv -Delimiter ';' -UseQuotes Never

#Copy CSV to right Folder
Copy-Item full_grouped.csv -Destination $AnimationFolder

Copy-Item full_grouped.csv -Destination $AnimationServiceFolder
Copy-Item time_series_covid19_confirmed_US.csv -Destination $AnimationServiceFolder
Copy-Item covid_19_clean_complete.csv -Destination $AnimationServiceFolder

#Import Corona data
Invoke-WebRequest -Uri http://localhost:12345/api/import -Method POST -Body ("corona_data"|ConvertTo-Json) -ContentType "application/json"

#Generate Animation
Invoke-WebRequest -Uri http://localhost:12345/api/animation -Method POST -Body ("generate,corona_spread"|ConvertTo-Json) -ContentType "application/json"