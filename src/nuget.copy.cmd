@echo off

copy Serialize.Linq\bin\Release\Serialize.Linq.dll nuget\net40\
copy Serialize.Linq.Net45\bin\Release\Serialize.Linq.dll nuget\net45\
copy Serialize.Linq.SL\Bin\Release\Serialize.Linq.SL.dll nuget\sl50\
copy Serialize.Linq.WP8\Bin\ARM\Release\Serialize.Linq.WP8.dll nuget\wp8\
copy Serialize.Linq.WP8\Bin\ARM\Release-ARM\Serialize.Linq.WP8.dll nuget\wp8-arm\
copy Serialize.Linq.NetCore11\bin\Release\netcoreapp1.1\Serialize.Linq.NetCore11.dll nuget\netcoreapp1.1\