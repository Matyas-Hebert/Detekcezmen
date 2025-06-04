# Stručný popis řešení a jeho případná omezení

## Aplikace
Aplikace slouží k analýze adresáře na vstupu a výpisu souborů jež byly přidány, smazány či změneny.

## Postup analýzy adresáře
1. Deserializace `files.json` souboru do objectu `FilesDictionary`
2. Rekurzivní analýza vstupního adresáře pomocí funkce `AnalyzeDirectory`.
3. Pro každý soubor v adresáři/podaresáři:
   - Pokud se nenachází v `FilesDictionary` objektu jedná se o nový souboru.
     Přídá se do seznamu `NewFiles` a uloží se do slovníku s verzí 1.
   - Pokud se soubor v `FilesDictioanry` nachází načte se obsah souboru, zavolá se pomocná hash funkce a vzniklý hash se porovná
     s hash uloženým v .json souboru. V případě že jsou hashe rozddílné jedná se o změněný soubor. Záznam o změněném souboru v .json souboru
     aktualizujeme (zvýšíme verzi o 1) a souboru přidáme do seznamu `ChangedFiles`.
4. Pro každý soubor v `FilesDictionary`, který se nachází v vstupním adresáři či jeho podadresářích ověříme zda existuje.
   V případě že soubor již neexistuje, byl soubor smazán. Přidáme ho do seznamu `DeletedFiles`.
5. Výsledky analýzy se uloží do objektu type `AnalyzeResult` a předají se do `Index.cshtml` view a zobrazí se v tabulce.
6. Změněný `FilesDictionary` se serializuje a uloží do `files.json`.

## Omezení řešení
1. Analýza může být pomalejší při větším počtu souborů, neboť při každém spuštění musí znovu načíst a zhashovat všechny souboru.
2. Změna souboru, která je později vrácena do původního stavu může být:
   - detekována jako změna, pokud byla mezitím provedena analýza
   - nedetekována vůbec pokudd k analýze došlo až po vrácení změn
3. V případě spuštění více instancí aplikace či zápisu do souborů při běhu analýzy může docházet k race-condition.
