# Lezione 7 — Draw call: batching, atlas e shader mobile

## Cosa costa davvero
Ogni **draw call** è una richiesta della CPU alla GPU ("disegna questa mesh con questo materiale"). Ogni **SetPass** è un cambio di shader/materiale, ancora più costoso. Su Quest la CPU è il collo di bottiglia più frequente: 500 draw call sono già troppe, 100–200 è una buona zona. Le leggi in Profiler → Rendering (anche sul visore) e in Frame Debugger (in editor: vedi ogni chiamata, e perché due oggetti non sono stati raggruppati).

## Le cinque leve
1. **Materiali condivisi.** Due oggetti con lo stesso materiale possono finire nello stesso batch; due copie dello stesso materiale no. Attenzione a `renderer.material` negli script: crea una copia per oggetto (usa `sharedMaterial`).
2. **SRP Batcher** (URP Asset → Advanced): raggruppa i cambi di materiale se lo shader è lo stesso. Deve stare acceso; verifica in Frame Debugger che le chiamate siano "SRP Batch".
3. **GPU instancing** (checkbox sul materiale): stessa mesh, stesso materiale, tante copie in una chiamata (sedie, libri, bottiglie).
4. **Static batching** (flag Batching Static, lezione 6): Unity combina le mesh statiche con lo stesso materiale.
5. **Atlas**: più texture in una, così più materiali diventano uno. **RUFA ▸ Strumenti ▸ Atlas dai materiali selezionati** crea la texture e il materiale; le UV delle mesh vanno rimappate nel rettangolo di ciascuna. Sui tuoi modelli lo fai in Blender; sulla casa del demo lo fa il docente, e la trovi già con l'atlas alla lezione dopo, con `git aggiorna`. Si perde il tiling: attenzione alla texel density (lezione 9).

## Shader mobile
`Simple Lit` (Blinn-Phong) costa molto meno di `Lit` (PBR) e di `Complex Lit`. Su Quest usa Simple Lit per quasi tutto, Lit solo dove il PBR si vede davvero (metalli, vetri). Anche mescolare shader diversi costa: ogni cambio di shader è una SetPass in più, e con le luci in tempo reale si ripete in ogni passata delle ombre. Meglio uno shader solo per quasi tutta la scena.

## Trasparenze e overdraw
Ogni superficie trasparente viene disegnata sopra a ciò che c'è dietro: cinque vetri sovrapposti = cinque volte gli stessi pixel. Rendering Debugger (Window → Analysis) → Overdraw mostra dove bruci fill rate. Rimedi: una sola lastra, alpha clip (cutout) per tende e fogliame, niente trasparenze a schermo pieno.

## Consegna
Riduzione misurata delle draw call nel tuo sandbox: tabella con Draw Calls, SetPass e Batches prima/dopo (Profiler → Rendering sul visore, stessa stazione), e le tre leve che hai usato.
