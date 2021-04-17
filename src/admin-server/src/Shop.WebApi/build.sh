#!/bin/bash

echo Linux Docker build ZIP

project_name="Shop.WebApi"

# release project
cd src/$project_name

rm -f $project_name.zip
rm -rf publish

dotnet publish -c Release -o publish

zip -r $project_name.zip publish