cd .. || exit
cd src || exit

modules=("Users" "Products" "Baskets")

for moduleName in "${modules[@]}" ; do
    cd "$moduleName" || exit
    cd "$moduleName".Core || exit
    dotnet ef migrations add "test-migration" -o Persistence/Migrations -s ../../API 
    dotnet ef database update -s ../../API
    cd ../..
done

 