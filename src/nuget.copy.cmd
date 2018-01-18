@echo off

robocopy Serialize.Linq\bin\Release\ nuget\net40\ Serialize.Linq.dll
robocopy Serialize.Linq.Net45\bin\Release\ nuget\net45\ Serialize.Linq.dll
robocopy Serialize.Linq.SL\Bin\Release\ nuget\sl50\ Serialize.Linq.SL.dll
robocopy Serialize.Linq.WP8\Bin\ARM\Release\ nuget\wp8\ Serialize.Linq.WP8.dll
robocopy Serialize.Linq.WP8\Bin\ARM\Release-ARM\ nuget\wp8-arm\ Serialize.Linq.WP8.dll
robocopy Serialize.Linq.NetCore11\bin\Release\netcoreapp1.1\ nuget\netcoreapp1.1\ Serialize.Linq.NetCore11.dll

robocopy Serialize.Linq.Universal\bin\Release\ nuget\uap10.0\ Serialize.Linq.Universal.dll
robocopy Serialize.Linq.Universal\bin\Release\ nuget\uap10.0\ Serialize.Linq.Universal.pri

robocopy Serialize.Linq.Universal\bin\ARM\Release\ nuget\runtimes\win10-arm\lib\ Serialize.Linq.Universal.dll
robocopy Serialize.Linq.Universal\bin\ARM\Release\ nuget\runtimes\win10-arm\lib\ Serialize.Linq.Universal.pri
robocopy Serialize.Linq.Universal\bin\x64\Release\ nuget\runtimes\win10-x64\lib\ Serialize.Linq.Universal.dll
robocopy Serialize.Linq.Universal\bin\x64\Release\ nuget\runtimes\win10-x64\lib\ Serialize.Linq.Universal.pri
robocopy Serialize.Linq.Universal\bin\x86\Release\ nuget\runtimes\win10-x86\lib\ Serialize.Linq.Universal.dll
robocopy Serialize.Linq.Universal\bin\x86\Release\ nuget\runtimes\win10-x86\lib\ Serialize.Linq.Universal.pri