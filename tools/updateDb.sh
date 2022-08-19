cd .. || exit
cd src || exit

modules=("Users" "Products" "Baskets")

for moduleName in "${modules[@]}" ; do
    cd "$moduleName" || exit
    cd "$moduleName".Core || exit 
    dotnet ef database update -s ../../AspNetCore
    cd ../..
done