Write-Host "F2020 publish start" -fore Green
Remove-Item 'C:\publish\Release\F2020' -Recurse -ErrorAction Ignore
cd ..
dotnet publish Konasoft.BV.Fonix3.F2020.csproj -c Release -o "C:\publish\Release\F2020" -f netcoreapp3.1
Write-Host "Backend kész" -fore Green
Remove-Item 'C:\publish\Release\F2020\wwwroot' -Recurse -ErrorAction Ignore
Remove-Item 'C:\publish\Release\F2020\F2020_UI' -Recurse -ErrorAction Ignore
cd "F2020_UI"
npm run build
Write-Host "Frontend kész" -fore Green
Copy-Item "dist" -Destination 'C:\publish\Release\F2020' -Recurse 
Rename-Item 'C:\publish\Release\F2020\dist' 'C:\publish\Release\F2020\wwwroot' 
start 'C:\publish\Release\F2020'
Write-Host 'Teljes build kész ' $(Get-Date -Format "yyyy.MM.dd. HH:mm:ss") -fore Green
Read-Host -Prompt 'Nyomj entert a bezáráshoz'

