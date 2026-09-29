# Sistemi interattivi – Game Art 3 (Modulo A) — Progetti Unity

Materiale del corso: il progetto **sandbox** su cui ogni studente costruisce il proprio walking simulator o la propria escape room, giocabile su Meta Quest 3S e su PC dallo stesso progetto Unity. Dentro c'è anche la casa del docente, la scena del demo, su cui si seguono le cure durante le dimostrazioni.

Ogni lezione: all'inizio `git aggiorna`, alla fine `git salva "Lezione NN"`, sempre con Unity chiuso (`materiali/01-setup.md`).

## Requisiti

- **Unity 6000.3.25f1** (Unity 6.3 LTS), installato da Unity Hub con il modulo **Android Build Support** e i sotto-moduli **OpenJDK** e **Android SDK & NDK Tools**.
- **Git** e **Git LFS** (`git lfs install`, una volta sola per macchina).
- **Meta Quest 3S** con modalità sviluppatore attiva, cavo USB-C, driver ADB su Windows.

## Primo avvio

1. Crea la tua copia di questo repository e clonala: i comandi sono in `materiali/01-setup.md`, sezione "Il tuo repository". Sulla copia lavori e consegni; da qui prendi solo gli aggiornamenti.
2. In Unity Hub: **Add → Add project from disk**, scegli la cartella `sandbox/` e aprila con 6000.3.25f1. La prima apertura scarica i package e importa tutto il progetto: 20–30 minuti, da lasciar finire senza toccare niente.
3. Nel Project apri la scena `Assets/Scenes/Sandbox.unity` (doppio clic). Il progetto è già configurato: non c'è niente da lanciare.
4. Premi **Play**: sei nella versione PC. `W A S D` per muoverti, mouse per guardare, tasto sinistro tenuto premuto per afferrare e portare (il mirino diventa giallo su ciò che si può prendere, verde sulla serratura dove lasciarlo), tasto destro per usare; `Esc` libera il cursore, click lo riprende.

## Build

- **RUFA ▸ Build ▸ Quest (APK)** → `Builds/Quest/RUFA_Sandbox.apk`. La prima volta l'editor cambia piattaforma e si ferma: rilancia la stessa voce di menu.
- Installazione sul visore (con il visore collegato via USB e l'autorizzazione al debug accettata):

      adb install -r "Builds/Quest/RUFA_Sandbox.apk"

- **RUFA ▸ Build ▸ PC (Windows)** → `Builds/PC/RUFA_Sandbox/RUFA_Sandbox.exe`.
- **RUFA ▸ Build ▸ Demo (APK)** → `Builds/Quest/RUFA_Demo.apk`: solo la casa del docente, come app a parte ("RUFA Demo"), per misurarla sul visore.
- **RUFA ▸ Play mode ▸ VR con Quest Link**: con la spunta, **Play** avvia la VR tramite Quest Link (serve l'app Meta Quest Link sul PC con runtime OpenXR impostato su Meta). Senza spunta, Play usa la versione PC.

Da riga di comando (esempio Windows, dalla cartella `sandbox/`):

    "C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Unity.exe" -batchmode -quit -projectPath . -buildTarget Android -executeMethod Rufa.Editor.Builds.BuildQuest -logFile build-quest.log
    "C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Unity.exe" -batchmode -quit -projectPath . -buildTarget Win64 -executeMethod Rufa.Editor.Builds.BuildPc -logFile build-pc.log

## I menu RUFA

- **Setup**: hanno preparato il progetto che cloni. 1 (impostazioni), 2 (scena), 3 (prefab mattoncino) e 4 (mini-esempi) sono già fatti e si rilanciano solo per rimettere a posto qualcosa: il 2 ricrea la scena da zero e il 3 riscrive i prefab. Il 5 (stazioni di benchmark) si lancia alla lezione 5, il 6 (vignetta e vibrazioni) alla lezione 12.
- **Build**: Quest (APK), PC (Windows), Quest (Development, Profiler) solo per profilare, e Demo (APK) per la casa del docente.
- **Play mode ▸ VR con Quest Link**.
- **Strumenti**: ognuno ripete su tanti oggetti un gesto che la dispensa della lezione fa fare prima a mano, nell'Inspector. Per lezione:
  - 6: Conta triangoli (selezione), LOD Group dalle varianti selezionate, Statici: imposta flag (selezione), Occlusion culling: bake / cancella;
  - 7: Atlas dai materiali selezionati;
  - 8: Lighting: preset mobile / bake / cancella, Light Probe Group a griglia (selezione), Reflection Probe (selezione);
  - 9: Texture: audit (selezione), Texture: preset mobile 2048 / 1024 (selezione);
  - 10: URP: preset Quest (asset selezionato), OpenXR: single pass e foveated rendering;
  - 11: Particle System: preset mobile (selezione);
  - 12: Audio: rendi spaziali (selezione), Reverb zone (selezione).

## Struttura

- `sandbox/Assets/Scenes/Sandbox.unity` — la scena di partenza: pavimento con area di teleport, un manichino alto 1,75 m e una porta da 80 × 210 cm come riferimenti di scala, e sotto `Mini-esempi` i due esempi della lezione 3.
- `sandbox/Assets/Demo/` — la casa del docente: scena, script dei difetti, materiali, texture e arredo (modelli CC0, crediti in `sandbox/Assets/Demo/Models/CREDITS.md`). Nel Play usa il suo livello di qualità, "Demo", con un asset URP pieno di difetti; le tue scene usano quello del progetto. A ogni `git aggiorna` torna com'è quella del docente.
- `sandbox/Packages/com.rufa.core/` — il nucleo che gestisce il doppio target (rig VR o rig PC, impostazioni, build). Non va modificato: il suo funzionamento viene spiegato alla lezione 13.
- `sandbox/Assets/Samples/XR Interaction Toolkit/…/Starter Assets/` — il rig VR e le azioni di input di XR Interaction Toolkit, importati dal menu Setup 1.

## Materiali del corso
- `materiali/00-mappa-progetto.md` — come funziona il sandbox: cosa c'è in scena, cosa nasce in Play, dove si cambiano le cose
- `materiali/01-setup.md` — setup, primo deploy, cosa fare se non va
- `materiali/02-locomozione.md` — locomozione e comfort
- `materiali/03-interazioni.md` — prefab mattoncino, eventi, problemi comuni
- `materiali/04-blockout.md` — scala, level design, concept e blockout
- `materiali/05-profiling.md` — profiling e frame budget
- `materiali/06-geometria.md` — poly count, LOD, culling
- `materiali/07-drawcall.md` — draw call, batching, atlas, shader
- `materiali/08-lighting.md` — illuminazione baked
- `materiali/09-texture.md` — texture e memoria
- `materiali/10-rendering.md` — rendering avanzato per Quest
- `materiali/11-vfx.md` — visual effects
- `materiali/12-audio-comfort.md` e `materiali/12-checklist-comfort.md` — audio, haptics, comfort
- `materiali/13-build-e-beta.md` — build pipeline e beta
- `materiali/14-scheda-playtest.md` — playtest
- `materiali/15-relazione-template.md` — relazione tecnica d'esame
- `materiali/checklist-consegne.md` — tutte le consegne
