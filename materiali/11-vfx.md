# Lezione 11 — Visual effects: Particle System, VFX Graph e la loro ottimizzazione

## Due sistemi
- **Particle System (Shuriken)**: simulato sulla CPU, disegnato come quad. Funziona ovunque, si controlla dall'Inspector, costa poco se le particelle sono poche e piccole. Su Quest si usa questo.
- **VFX Graph**: simulato sulla GPU con compute shader, gestisce milioni di particelle, si costruisce a nodi. Ottimo su PC; su Quest ha limiti e overhead. Nella scena del demo, dalla lezione 11, lo vedi in Play su PC (l'oggetto `Polvere VFX`, nel Salotto), non sul visore.

## Il vero costo: i pixel
Cento particelle grandi come lo schermo costano più di diecimila particelle piccole. Ogni quad trasparente viene disegnato sopra ciò che c'è dietro (overdraw); il fill rate del Quest è limitato. Guarda Rendering Debugger → Overdraw: dove è bianco stai pagando.

## Regole per Quest
1. Particelle **piccole** (0,05–0,3 m) e **poche** (max 50–100 per sistema).
2. **Alpha blend** invece di additive quando non serve il bagliore; niente **soft particles** (leggono la depth texture: una copia dello schermo per frame).
3. Texture **piccole con mipmap** (128–256 px bastano per un disco morbido).
4. **Culling automatico** (Culling Mode → Automatic): il sistema si mette in pausa fuori dalla vista.
5. Niente luci sulle particelle, niente trail, niente collisioni.
6. Effetti ambientali che rendono molto e costano poco: polvere nei raggi di luce, poche foglie che cadono, fuoco con uno sprite sheet animato, nebbia bassa fatta con tre o quattro quad grandi ma fermi.

**RUFA ▸ Strumenti ▸ Particle System: preset mobile (selezione)** applica i limiti 1 e 4 e toglie luci e trail.

## Creare un effetto
1. **GameObject → Effects → Particle System**, e mettilo dove serve (per la polvere: nel raggio di luce di una finestra).
2. Materiale: **Create → Material** con shader **Universal Render Pipeline/Particles/Unlit**, Surface Type Transparent, Blending Mode Alpha, **Soft Particles spento**; come Base Map una texture piccola (128–256 px) con mipmap. Trascinalo in **Renderer → Material**.
3. Nel modulo principale: **Start Size** (0,05–0,3), **Start Lifetime**, **Start Speed** bassa per la polvere, **Max Particles**; in **Emission** il numero di particelle al secondo; in **Shape** il volume in cui nascono (Box per una stanza, Cone per un getto).
4. Seleziona l'oggetto e lancia il **preset mobile**; poi misura.

## Misurare
Profiler → CPU Usage (cerca `ParticleSystem` nella gerarchia: è il tempo di simulazione), GPU ms e overdraw. Cambia una cosa alla volta e lancia il benchmark.

## Consegna
Effetti ambientali integrati nell'atmosfera del tuo progetto, ottimizzati con le regole sopra, con misura prima/dopo. Facoltativo: un esperimento con VFX Graph in un progetto a parte, non VR (template Universal 3D più il package **Visual Effect Graph**, poi Create → Visual Effects → Visual Effect Graph). Nel sandbox no: finirebbe anche nella build per il visore.
