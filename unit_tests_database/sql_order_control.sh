#!/usr/bin/env bash

# Some SQL references other SQL, as such clear run order is required.
set -e 

function create_object_prefix_map {
    local -n arr_ref=$1
    local object_types=("${!2}")
    local lower_a=97
    local i=0
    for type in "${object_types[@]}"; do
        char_numb=$(($lower_a + $i))
        ascii_character=$(printf "\\$(printf '%03o' $char_numb)")
        arr_ref["$type"]=$ascii_character
        i=$((i + 1))
    done
}

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

function unpack_object_scripts {
    local object_type=$1
    local -n prefix_map_ref=$2
    for filename in $object_type/*.sql; do
        mv {$object_type/,${prefix_map_ref[$object_type]}_}"$(basename $filename)";
    done
    rm -r $object_type
}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd $SCRIPT_DIR

db_object_types=( tables functions views stored_procedures )

declare -A object_prefixes
create_object_prefix_map object_prefixes db_object_types[@]


relative_table_order=$( get_sql_script_order "./table_script_order.dat" )

order_dependent_scripts "tables" $relative_table_order

unpack_object_scripts "tables" object_prefixes
unpack_object_scripts "functions" object_prefixes
unpack_object_scripts "views" object_prefixes
unpack_object_scripts "stored_procedures" object_prefixes

# prevent being triggered by base image
rm -f $(basename "$0")
