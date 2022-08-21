ls
cd .. || exit
cd src || exit

dotnet tool install --global dotnet-ef
dotnet tool restore

modules=("Users" "Products" "Baskets")

for moduleName in "${modules[@]}" ; do
    cd "$moduleName" || exit
    cd "$moduleName".Core || exit
    dotnet ef migrations add "test-migration" -o Persistence/Migrations -s ../../API -c "$moduleName"DbContext
    dotnet ef database update -s ../../API -c "$moduleName"DbContext
    cd ../..
done

 