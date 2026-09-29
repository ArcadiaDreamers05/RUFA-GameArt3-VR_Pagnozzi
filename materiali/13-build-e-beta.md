# Lezione 13 — Build pipeline PC e VR, consegna della beta

## Le build
Dallo stesso progetto escono due build:
- **RUFA ▸ Build ▸ Quest (APK)** → `Builds/Quest/<nome>.apk`, da installare con `adb install -r`.
- **RUFA ▸ Build ▸ PC (Windows)** → `Builds/PC/<nome>/<nome>.exe`.
- **RUFA ▸ Build ▸ Quest (Development, Profiler)** → solo per profilare: più lenta, non è la beta.
La prima volta che cambi piattaforma l'editor si ferma: rilancia la stessa voce. Da riga di comando: vedi il README del progetto.

## Versioning
Project Settings → Player: **Version** (es. `0.9.0` per la beta, `1.0.0` per l'esame) e, in Android → Other Settings, **Bundle Version Code** (un intero: alzalo a ogni build che installi, altrimenti `adb install -r` può rifiutare). Il **Package Name** deve essere tuo (`it.rufa.gameart3.<cognome>`), così il tuo progetto convive con gli altri sullo stesso visore.

## Dietro le quinte del doppio target (per chi vuole)
All'avvio `RigBootstrap` chiede a XR Plug-in Management se c'è un loader attivo: sul Quest sì (la build Android inizializza OpenXR all'avvio), su PC no (la build Windows non lo fa). Con il loader attivo istanzia il rig VR, altrimenti costruisce il rig desktop. Gli interactable sono gli stessi: il rig desktop usa un `XRRayInteractor` alimentato dal mouse. Le impostazioni per piattaforma stanno negli asset URP e nei livelli di qualità.

## Cosa consegnare alla lezione 13 (milestone beta)
Nella cartella `Consegne/L13/` del tuo repository: l'APK, la build PC (zip), il CSV del benchmark della build non development, la checklist qui sotto compilata.

## Checklist della beta
| # | Voce | Sì/No |
|---|---|---|
| 1 | L'APK si installa e parte senza errori sul Quest 3S | |
| 2 | La scena è completa: si può percorrere dall'inizio alla fine | |
| 3 | Locomozione configurata (movimento, snap turn, teleport dove serve) | |
| 4 | Almeno un enigma (escape room) o un trigger narrativo (walking simulator) funzionante | |
| 5 | Audio: ambience e almeno una sorgente spaziale | |
| 6 | 90 Hz sul benchmark (build non development) | |
| 7 | La build PC parte e si gioca con WASD e mouse | |
| 8 | Tutti gli asset hanno licenza CC0 o equivalente, elencati in `CREDITS.md` | |
| 9 | Product Name e Package Name tuoi | |
| 10 | Version e Bundle Version Code alzati | |
| 11 | In `Consegne/L13/README.md` c'è come aprire e buildare il progetto (il README della radice lo aggiorna il corso) | |
| 12 | Tutto è committato e pushato (LFS per i binari) | |
