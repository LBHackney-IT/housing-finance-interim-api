#!/usr/bin/env bash

# Some SQL references other SQL, as such clear run order is required.
set -e 

function get_sql_script_order {
    local file_path=$1
    if [ -f "$file_path" ]; then
        grep -iP "^\w+$" $file_path
    else
        echo ""
    fi
}

function order_dependent_scripts {
    local object_type=$1
    shift
    local relative_order=("$@")
    local i=0
    for file in "${relative_order[@]}"; do
        i=$((i + 1))
        padded_i=$(printf "%02d" "$i")
        mv "${object_type}/$file.sql" "${object_type}/${padded_i}_${file}.sql"
    done
}

declare -A object_prefixes
object_prefixes[tables]="a"
object_prefixes[functions]="b"
object_prefixes[views]="c"
object_prefixes[stored_procedures]="d"

function unpack_object_scripts {
    local object_type=$1
    for filename in $object_type/*.sql; do
        mv {$object_type/,${object_prefixes[$object_type]}_}"$(basename $filename)";
    done
    rm -r $object_type
}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd $SCRIPT_DIR

relative_table_order=$( get_sql_script_order "./table_script_order.dat" )

order_dependent_scripts "tables" $relative_table_order

unpack_object_scripts "tables"
unpack_object_scripts "functions"
unpack_object_scripts "views"
unpack_object_scripts "stored_procedures"

# prevent being triggered by base image
rm -f $(basename "$0")
