# Lezione 1 — Setup e primo deploy: la procedura

## Cosa serve sul PC
- **Unity Hub** con **Unity 6000.3.25f1** e il modulo **Android Build Support** (con OpenJDK e Android SDK & NDK Tools). L'installazione dei moduli chiede spesso i permessi di amministratore: fallo prima della lezione.
- **Git** con **Git LFS** (`git lfs install` una volta sola).
- **ADB**: è già dentro Unity, in `C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe`. Aggiungi quella cartella al PATH oppure usa il percorso completo.

## Cosa serve sul visore
- **Modalità sviluppatore** attiva. Si accende dall'app **Meta Horizon** sul telefono: Menu → Dispositivi → il tuo visore → Impostazioni del visore → Modalità sviluppatore. Funziona solo se l'account Meta del visore fa parte di un'organizzazione sviluppatore verificata: in laboratorio è già così, a casa devi registrarti su developer.oculus.com.
- Cavo USB-C al PC. Alla prima connessione il visore mostra "Consenti debug USB": accetta e spunta "Consenti sempre da questo computer".

## Il tuo repository
Il repository del corso è in sola lettura: lavori su una tua copia, sul tuo account GitHub. È anche il posto delle consegne (`Consegne/Lxx/`).

1. Su github.com: **New repository**, un nome (per esempio `rufa-vr-cognome`), **Private**, niente README, .gitignore o licenza, **Create repository**. Copia l'indirizzo che finisce con `.git`.
2. Nel terminale, nella cartella dove tieni i progetti. Meglio un percorso corto, per esempio `C:\Users\<nome>`: Unity non regge i percorsi troppo lunghi.

       git clone https://github.com/Gianluca-C90/RUFA-GameArt3-VR.git rufa-vr
       cd rufa-vr
       git config user.name "Nome Cognome"
       git config user.email "la-tua-email@esempio.it"
       git remote rename origin corso
       git remote add origin <l'indirizzo del tuo repository>
       git config include.path ../.gitconfig
       git config lfs.remote.searchall true
       git lfs fetch --all corso
       git push -u origin main

   Nelle righe `user.name` e `user.email` metti il tuo nome e l'email del tuo account GitHub: firmano i tuoi salvataggi. Valgono solo per questa copia, perché i PC del laboratorio sono di tutti.

   Da qui in poi il lavoro va sul tuo repository. Quello del corso si chiama `corso`, e da lì prendi solo gli aggiornamenti. La riga `include.path` attiva i due comandi del corso, `git aggiorna` e `git salva` (sezione "Inizio e fine lezione").
3. Sul tuo repository: **Settings → Collaborators → Add people** → `Gianluca-C90`, così il docente vede le consegne.

## Su un altro PC
A casa, o su un altro PC del laboratorio, la tua copia si prende dal **tuo** repository e poi si ricollega il corso:

       git clone <l'indirizzo del tuo repository> rufa-vr
       cd rufa-vr
       git config user.name "Nome Cognome"
       git config user.email "la-tua-email@esempio.it"
       git remote add corso https://github.com/Gianluca-C90/RUFA-GameArt3-VR.git
       git config include.path ../.gitconfig
       git config lfs.remote.searchall true
       git fetch corso

Se lavori su due PC, prima di cominciare su uno prendi quello che hai salvato sull'altro: `git pull`.

## Aprire il sandbox
1. In Unity Hub **Add → Add project from disk** e scegli la cartella `sandbox/` della tua copia. La prima apertura scarica i package e importa tutto il progetto, demo compreso: 20–30 minuti. Lasciala finire senza toccare niente.
2. Nel Project apri `Assets/Scenes/Sandbox.unity` (doppio clic): pavimento, manichino da 1,75 m, porta da 80 × 210 cm e, sotto `Mini-esempi`, i due esempi della lezione 3. Il progetto è già configurato (Player, XR, URP): non c'è niente da lanciare.
   L'altra scena, `Assets/Demo/Scenes/Demo.unity`, è la casa del docente: la stessa che vedi in dimostrazione, con tutti i suoi difetti. Durante le dimostrazioni fai le cure su questa, insieme al docente.
3. **Play**: sei nella versione PC. `W A S D` o frecce per muoverti, mouse per guardare, `Esc` libera il cursore, click lo riprende. Nella Hierarchy compaiono `XR Interaction Manager` e `Desktop Rig`: li crea il progetto all'avvio, non cercarli nella scena salvata.

## Build e deploy su Quest
1. **RUFA ▸ Build ▸ Quest (APK)**. La prima volta l'editor cambia piattaforma e si ferma con `Run the same menu item again to build.`: rilancia la stessa voce. La prima build IL2CPP è lunga, anche più di un'ora a seconda del PC: lasciala andare, le successive durano pochi minuti. L'esito è in Console (`Succeeded`) e il file è `Builds/Quest/RUFA_Sandbox.apk`.
2. Visore collegato, dalla cartella `sandbox/`:

       adb devices
       adb install -r "Builds/Quest/RUFA_Sandbox.apk"

   `adb devices` deve elencare il visore come `device`. Se dice `unauthorized`, indossa il visore e accetta la richiesta di debug; se non compare nulla, cambia cavo o porta USB.
3. Sul visore: **Libreria → Origini sconosciute → RUFA Sandbox**. Devi vedere pavimento, manichino e porta a grandezza naturale, i controller, lo stick sinistro che muove e quello destro che ruota a scatti.

## Build PC
**RUFA ▸ Build ▸ PC (Windows)** (anche qui, la prima volta rilancia) → `Builds/PC/RUFA_Sandbox/RUFA_Sandbox.exe`. Si comporta come il Play in editor.

## Inizio e fine lezione
Due comandi, dalla cartella della tua copia (`rufa-vr`), sempre con **Unity chiuso**:

- **Inizio lezione:** `git aggiorna`. Prende dal corso il nucleo, le dispense e la cartella del demo (`Assets/Demo`), e li salva sul tuo repository. La cartella del demo torna identica a quella del docente, anche se hai saltato una lezione. Il tuo progetto non lo tocca.
- **Fine lezione:** in Unity Ctrl+S e **File ▸ Save Project**, poi chiudi Unity e lancia `git salva "Lezione 03"`. Salva tutto, consegna in `Consegne/L03/` compresa, e lo manda sul tuo repository.

Perché gli aggiornamenti non rompano mai niente:
- il tuo lavoro va in `Assets/Progetto/` e nelle tue scene. Nella cartella `Assets/Demo` lavori solo durante le dimostrazioni: a ogni `git aggiorna` torna quella del docente, e la tua versione resta nella storia di git;
- `sandbox/Packages/com.rufa.core/` e `materiali/` non si modificano: li aggiorna il corso. Se li hai toccati per sbaglio, `git aggiorna` li rimette com'erano;
- il demo usa anche file del tuo progetto, che `git aggiorna` non tocca: i prefab mattoncino di `Assets/RUFA/Prefabs`, il rig `XR Origin (XR Rig)` e le impostazioni URP di `Assets/Settings`. Se li modifichi, cambiano anche nel demo. Per le tue versioni dei mattoncini fai una copia, o una Prefab Variant, in `Assets/Progetto`;
- per usare nel tuo progetto un modello del demo, duplicalo (Ctrl+D) e sposta la copia in `Assets/Progetto`. Se sposti l'originale, `git aggiorna` lo rimette al suo posto e Unity si ritrova due file con lo stesso GUID;
- non salvare scene o file tuoi dentro `Assets/Demo`: `git aggiorna` li cancella, e lo scrive (`Removing …`).

## Il menu RUFA
Le voci del menu **RUFA** sono di quattro tipi:
- **Setup**: hanno preparato il progetto che hai clonato (impostazioni, scena, prefab mattoncino, mini-esempi). Per lavorare non servono. Si rilanciano solo per rimettere a posto qualcosa, e attenzione: il 2 ricrea la scena da zero, il 3 riscrive i prefab. Il 5 (stazioni di benchmark) e il 6 (vignetta e vibrazioni) li lanci alle lezioni 5 e 12.
- **Build**: le build per Quest e per PC.
- **Play mode**: il Play in editor da PC oppure in VR con Quest Link.
- **Strumenti**, dalla lezione 6: ognuno ripete su tanti oggetti un gesto che la dispensa ti fa fare prima a mano, nell'Inspector.

Il doppio target, cioè lo stesso progetto in VR e su PC, sta nel package `com.rufa.core`: non va modificato, e come funziona si vede alla lezione 13.

## Se qualcosa non va
| Sintomo | Causa e rimedio |
|---|---|
| All'apertura la scena è vuota (`Untitled`) | Unity non sa ancora quale scena aprire: doppio clic su `Assets/Scenes/Sandbox.unity` |
| Alla prima apertura, errori `DirectoryNotFoundException` su file dentro `Library\PackageCache` | Il percorso della cartella è troppo lungo per Unity: sposta la copia in una cartella più corta (per esempio `C:\Users\<nome>\rufa-vr`), cancella `sandbox\Library` e riapri |
| Impostazioni di Player, XR o URP cambiate per sbaglio | **RUFA ▸ Setup ▸ 1** le riapplica tutte |
| Unity chiede di importare "TMP Essentials" | Accetta: serve per i testi |
| `git: 'aggiorna' is not a git command` | Manca il collegamento ai comandi del corso: dalla cartella della tua copia `git config include.path ../.gitconfig` |
| `Author identity unknown` o `Please tell me who you are` | Su questa copia mancano nome ed email: dalla sua cartella `git config user.name "Nome Cognome"` e `git config user.email "la-tua-email@esempio.it"`, poi di nuovo il comando |
| Il primo `git push -u origin main` risponde `rejected` | Su GitHub il repository è stato creato con un README. Solo questa volta: `git push -u --force origin main` |
| `git aggiorna` o `git salva` rispondono `rejected` | Il tuo repository ha lavoro che questa copia non ha, per esempio salvato da un altro PC: `git pull --no-rebase --no-edit`, poi di nuovo il comando |
| `git salva` ha dato un errore, per esempio di rete | Rilancialo: se il salvataggio c'è già, lo manda soltanto |
| `git push` risponde 403 o "Permission denied" | Il PC del laboratorio ricorda l'account GitHub di un altro: `git credential-manager github list` per vederlo, `git credential-manager github logout <utente>` per toglierlo, poi di nuovo `git push` |
| `INSTALL_FAILED_UPDATE_INCOMPATIBLE` | Sul visore c'è la stessa app, fatta da un altro PC: `adb uninstall it.rufa.gameart3.sandbox` (per RUFA Demo: `adb uninstall it.rufa.gameart3.demo`), poi reinstalla |
| L'app non compare in Origini sconosciute | La modalità sviluppatore non è attiva, oppure l'install è fallito: rileggi l'output di `adb install` |
| Nel visore la scena è nera o gira male | Non chiudere: apri `adb logcat -s Unity` e copia gli errori nella consegna, li vediamo insieme |
| Il visore non è disponibile (batteria, cavo, permessi) | Consegna comunque la build PC funzionante e la Console pulita; il deploy si recupera all'inizio della lezione 2, in aula |

## Consegna
In `Consegne/L01/` del tuo repository: uno screenshot (o foto) del sandbox sul visore, uno screenshot della Console senza errori dopo il Play in editor. Se il deploy non è riuscito, al posto della foto metti l'output di `adb install` e di `adb devices`.
