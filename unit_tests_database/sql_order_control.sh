#!/usr/bin/env bash

# Some SQL references other SQL, as such clear run order is required.
set -e 


SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd $SCRIPT_DIR


declare -A object_prefixes
object_prefixes[tables]="a"
object_prefixes[functions]="b"
object_prefixes[views]="c"
object_prefixes[stored_procedures]="d"

relative_table_order=(
    # batch log group
    batch_log
    batch_log_error

    # charges group
    charges
    charges_history

    # direct debit group
    direct_debit_history
    direct_debit_suspense_accounts

    # cash load group
    "up_cash_dump_file_name"
    "up_cash_dump"
    "up_cash_load"
    "up_cash_load_suspense_accounts"

    # housing cash load group
    "up_housing_cash_dump_file_name"
    "up_housing_cash_dump"
    "up_housing_cash_load"
    "up_housing_cash_load_suspense_accounts"
)

i=0
for file in "${relative_table_order[@]}"; do
    i=$((i + 1))
    padded_i=$(printf "%02d" "$i")
    mv "tables/$file.sql" "tables/${padded_i}_${file}.sql"
done

for filename in tables/*.sql; do mv {tables/,${object_prefixes[tables]}_}"$(basename $filename)"; done;
for filename in functions/*.sql; do mv {functions/,${object_prefixes[functions]}_}"$(basename $filename)"; done;
for filename in views/*.sql; do mv {views/,${object_prefixes[views]}_}"$(basename $filename)"; done;
for filename in stored_procedures/*.sql; do mv {stored_procedures/,${object_prefixes[stored_procedures]}_}"$(basename $filename)"; done;

rm -r tables functions views stored_procedures

rm -r tables
