#!/bin/sh
set -e

print_help() {
  printf "\n"
  printf "Usage: %s <URL> <FILE> [<FILE> ... <FILE>]\n" "$(basename $0)"
  printf "\t<URL>\t\tUrl of the signer function\n"
  printf "\t<FILE>\tFile(s) to send to the signer function. They will be replaced with results from signer function\n"
  printf "\n"
  printf "Example:\n"
  printf "\t%s \"%s\" %s\n" "$(basename $0)" "https://scriptsigner.azurewebsites.net/api/SignEXE?code=blah" "./path/to/binary/*.exe"
  printf "\n"
}

sign_file() {
  FILE="$1"
  OUTPUT_FILE="$FILE-signed"
  printf "Signing %s and saving result to %s\n" "$FILE" "$OUTPUT_FILE"
  # Send FILE to signer function and save result to OUTPUT_FILE
  HTTP_CODE=$(curl --silent "$URL" -X POST -H "Accept: application/octet-stream" -H "Content-Type: application/octet-stream" -F signme=@"$FILE" -o "$OUTPUT_FILE" -w "%{http_code}")

  # If response from signer function indicates non-success, show error and clean up temp file
  if [ $HTTP_CODE -lt 200 ] || [ $HTTP_CODE -gt 299 ]; then
    printf "FAILED: Request failed with http code %s; Response body:\n%s\n" $HTTP_CODE "$(test -s "$OUTPUT_FILE" && cat "$OUTPUT_FILE" || printf "*NO DATA*")"
    rm "$OUTPUT_FILE"
    return 1
  fi

  # If response from signer is empty, show error and clean up temp file
  if [ ! -s "$OUTPUT_FILE" ]; then
    printf "FAILED: Signer function did not return anything\n"
    rm "$OUTPUT_FILE"
    return 1
  fi

  printf "Request succeeded (http code: %s); Moving %s to %s\n" $HTTP_CODE "$OUTPUT_FILE" "$FILE"
  mv "$OUTPUT_FILE" "$FILE"
  # we will go ahead and ZIP all binaries, since these will usually be large and should be shipped compressed
  printf "Moved file. Compressing %s...\n" "$FILE"
  zip -j -9 "$FILE.zip" "$FILE"
  printf "DONE: Compressed file.\n"
}

# if less that two args are provided, print help and exit
(test -z "$1" || test -z "$2") && print_help && exit 1

# save first arg as url to azure function
URL="$1"

# remove first arg - all remaining args will be considered as paths to script files
shift

# loop over all provided script files and sign them
for e in "$@"; do
  if [ ! -f "$e" ]; then
    printf "File cannot be found: %s\n" "$e"
  else
    sign_file "$e"
  fi
done
