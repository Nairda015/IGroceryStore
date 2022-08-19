cd .. || exit
cd src || exit

echo "Migration name:"
read -r migrationName

modules=("Users" "Products" "Baskets")

for moduleName in "${modules[@]}" ; do
    cd "$moduleName" || exit
    cd "$moduleName".Core || exit
    dotnet ef migrations add "$migrationName" -o Persistence/Migrations -s ../../AspNetCore 
    dotnet ef database update -s ../../AspNetCore
    cd ../..
done

 