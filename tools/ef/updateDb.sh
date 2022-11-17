cd .. || exit
cd src || exit

modules=("Users" "Products")

for moduleName in "${modules[@]}" ; do
    cd "$moduleName" || exit
    cd "$moduleName".Core || exit
    dotnet ef database update -s ../../API -c "$moduleName"DbContext
    cd ../..
done