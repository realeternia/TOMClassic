ExcelToCsv.exe
cd .\ConfigData
csc /target:library /r:../NarlonLib.dll /r:../BaseType.dll /out:../ConfigData.dll *.cs
cd ..
copy "./ConfigData.dll" "../TaleofMonsters2/Lib/"