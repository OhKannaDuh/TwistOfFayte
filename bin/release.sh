#!/bin/bash
set -e

if [ -z "$1" ]; then
    echo "Usage: $0 <tag> [--testing]"
    exit 1
fi

TAG="$1"
IS_TESTING=false

if [ "$2" == "--testing" ]; then
    IS_TESTING=true
fi

echo "Tag: $TAG"
echo "Testing release: $IS_TESTING"

# Create tag and release
git tag "$TAG"
git push origin master "$TAG"
dotnet build -c Release

if [ "$IS_TESTING" = true ]; then
    gh release create "$TAG" --title "$TAG" --generate-notes --prerelease
else
    gh release create "$TAG" --title "$TAG" --generate-notes
fi

gh release upload "$TAG" TwistOfFayte/bin/x64/Release/TwistOfFayte/latest.zip --clobber

# Update plugin manifest
 gh repo clone plugins
 cd plugins
 npm install

 manifest_output=$(node generate_manifest.js)
 commit_message=$(echo "$manifest_output" | awk '/^Suggested commit message:/{getline; print}')

 git add manifest.json
 git commit -m"$commit_message"
 git push origin master

 node generate_discord_message.js

 cd ..
 rm -rf plugins
