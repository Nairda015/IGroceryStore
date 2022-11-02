cd .. || exit
cd src || exit

echo "Migration name:"
read -r migrationName

modules=("Users" "Products")

for moduleName in "${modules[@]}" ; do
    cd "$moduleName" || exit
    cd "$moduleName".Core || exit
    dotnet ef migrations add "$migrationName" -o Persistence/Migrations -s ../../API -c "$moduleName"DbContext
    dotnet ef database update -s ../../API -c "$moduleName"DbContext
    cd ../..
done

 