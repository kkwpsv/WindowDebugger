for /f %%i in ('grep -oP "(?<=<Version>).*(?=</Version>)" WindowDebugger/WindowDebugger.csproj') do set Version= %%i

if "%Version%"=="" exit -1

for /f %%i in ('git tag ^| grep %Version%') do set IsExist= %%i

if "%IsExist%"=="" (
::not exist
curl --ssl-no-revoke -H "JOB-TOKEN: %CI_JOB_TOKEN%" -H "Content-Type: application/json" -d "{ """name""": """%Version%""", """tag_name""": """%Version%""", """ref""": """%CI_BUILD_REF%"""}" "%CI_API_V4_URL%/projects/%CI_PROJECT_ID%/releases"
7z a WindowDebugger.netcoreapp3.1.7z .\WindowDebugger\bin\Release\netcoreapp3.1\*
::TODO Upload to somewhere

) else (
::exist
exit 0
)