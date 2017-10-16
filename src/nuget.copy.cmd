@echo off

copy Serialize.Linq\bin\Release\Serialize.Linq.dll nuget\net40\
copy Serialize.Linq.Net45\bin\Release\Serialize.Linq.dll nuget\net45\
copy Serialize.Linq.SL\Bin\Release\Serialize.Linq.SL.dll nuget\sl50\
copy Serialize.Linq.WP8\Bin\ARM\Release\Serialize.Linq.WP8.dll nuget\wp8\
copy Serialize.Linq.WP8\Bin\ARM\Release-ARM\Serialize.Linq.WP8.dll nuget\wp8-arm\
copy Serialize.Linq.NetCore11\bin\Release\netcoreapp1.1\Serialize.Linq.NetCore11.dll nuget\netcoreapp1.1\

copy Serialize.Linq.Universal\bin\Release\Serialize.Linq.Universal.dll nuget\uap10.0\
copy Serialize.Linq.Universal\bin\Release\Serialize.Linq.Universal.pri nuget\uap10.0\

copy Serialize.Linq.Universal\bin\ARM\Release\Serialize.Linq.Universal.dll nuget\runtimes\win10-arm\lib\
copy Serialize.Linq.Universal\bin\ARM\Release\Serialize.Linq.Universal.pri nuget\runtimes\win10-arm\lib\
copy Serialize.Linq.Universal\bin\x64\Release\Serialize.Linq.Universal.dll nuget\runtimes\win10-x64\lib\
copy Serialize.Linq.Universal\bin\x64\Release\Serialize.Linq.Universal.pri nuget\runtimes\win10-x64\lib\
copy Serialize.Linq.Universal\bin\x86\Release\Serialize.Linq.Universal.dll nuget\runtimes\win10-x86\lib\
copy Serialize.Linq.Universal\bin\x86\Release\Serialize.Linq.Universal.pri nuget\runtimes\win10-x86\lib\