# Lezione 5 — Profiling e frame budget: la guida pratica

## Il budget
Il Quest 3S aggiorna lo schermo 90 volte al secondo: ogni frame ha **11,1 ms**. Se CPU o GPU ci mettono di più, il visore salta frame e si vede judder. Ottimizzare vuol dire sapere **chi** sfora (CPU o GPU) e **perché**, prima di toccare qualsiasi cosa.

## CPU-bound o GPU-bound?
- **CPU-bound**: il frame è lento anche abbassando la risoluzione (render scale 0,5 non cambia nulla). Cause tipiche: troppe draw call, troppi oggetti, script pesanti, fisica.
- **GPU-bound**: abbassando la risoluzione il frame time scende. Cause tipiche: troppi pixel (overdraw, trasparenze, post-processing), shader pesanti, luci realtime con ombre, texture enormi.
- Il modo più sicuro per saperlo: nel Profiler aggiungi il modulo **Highlights** (pulsante *Profiler Modules* in alto a sinistra), che dice direttamente se il frame è limitato dalla CPU o dalla GPU. In alternativa confronta `cpu_mean_ms` e `gpu_mean_ms` nel CSV del benchmark: il più alto dei due è il collo di bottiglia.

## Unity Profiler collegato al visore
1. **RUFA ▸ Build ▸ Quest (Development, Profiler)** e installa `Builds/Quest/<nome>_dev.apk` con `adb install -r`.
2. Visore collegato via USB: `adb forward tcp:34999 localabstract:Unity-<identifier>` (per il sandbox: `Unity-it.rufa.gameart3.sandbox`). In alternativa Wi-Fi: PC e visore sulla stessa rete e "Autoconnect Profiler" attivo nella build.
3. Window → Analysis → **Profiler**; in alto scegli il target `AndroidPlayer` (o `<Enter IP>`).
4. Moduli da guardare: **Highlights** (CPU o GPU?), **CPU Usage** (cliccando un frame, la *Hierarchy* in basso mette in cima ciò che costa di più), **GPU Usage** (se disponibile), **Rendering** (draw call, SetPass, triangoli, vertici), **Memory** (texture, mesh).
5. Ricorda: la build di profiling è più lenta. I numeri "veri" li prendi con la build normale.

## Frame Debugger (solo in editor o su PC)
Window → Analysis → **Frame Debugger** → Enable. Scorri le draw call: vedi cosa disegna ogni chiamata, quante SetPass, in che ordine, e perché due oggetti non vengono raggruppati (materiale diverso, shader diverso, trasparenza).

## OVR Metrics Tool (sul visore)
Installa **Meta Quest Developer Hub** sul PC, collega il visore, in Device Manager → Tools attiva **OVR Metrics Tool** e l'overlay persistente. Nel visore leggi FPS, App GPU Time, GPU %, CPU %: il modo più rapido per vedere l'effetto di una modifica.

## Il benchmark del progetto
Il sandbox ha tre stazioni di misura (dischi con freccia): le crea **RUFA ▸ Setup ▸ 5 - Crea stazioni di benchmark**, poi salva la scena. Per misurare:
- **PC**: in Play o nella build premi `B`: il rig visita le stazioni e scrive un file CSV in `%USERPROFILE%\AppData\LocalLow\RUFA\RUFA Sandbox\`.
- **Il demo**: la scena del demo ha già le sue stazioni. Per misurarlo sul visore, lo stesso `Start On Launch` sul suo `Benchmark`, poi **RUFA ▸ Build ▸ Demo (APK)**: diventa un'app a parte, "RUFA Demo".
- **Visore**: attiva `Start On Launch` sul componente `Benchmark` (oggetto `Benchmark` nella scena), fai la build normale, indossa il visore, vai su ogni disco e guarda la freccia; a fine giro il CSV è in `/sdcard/Android/data/it.rufa.gameart3.sandbox/files/` (per RUFA Demo: `/sdcard/Android/data/it.rufa.gameart3.demo/files/`) e lo scarichi con `adb pull`.
Il CSV riporta per ogni stazione: frame time medio, 1% low (media dei frame peggiori), massimo, fps, tempo CPU e GPU.

Per aprirlo: il file usa la virgola tra le colonne e il punto per i decimali. Con Excel in italiano usa **Dati → Da testo/CSV** e imposta l'origine del file su *Stati Uniti* (altrimenti 343.41 diventa un numero sbagliato); più semplice aprirlo con Fogli Google o LibreOffice Calc scegliendo la lingua inglese nell'importazione.

## Il report di profiling (consegna di oggi)
Una pagina con: (1) piattaforma e build usata; (2) la tabella del benchmark, stazione per stazione; (3) tre screenshot del Profiler o di OVR Metrics con una frase ciascuno; (4) la diagnosi: CPU-bound o GPU-bound, e le tre cause principali che pensi di dover curare nelle prossime lezioni.
