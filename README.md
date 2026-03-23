# MediaTekDocuments

Application de bureau Windows Forms (.NET Framework 4.7.2) permettant de gérer les documents d'une médiathèque : livres, DVD et revues. Elle communique avec une API REST PHP pour toutes les opérations sur la base de données MySQL.

---

## Présentation

Ce dépôt est un fork du dépôt d'origine :
**https://github.com/CNED-SLAM/MediaTekDocuments**

Le README du dépôt d'origine présente la version initiale de l'application : structure des onglets, schéma de la base de données, fonctionnement de la consultation du catalogue et de la réception des parutions de revues. Les fonctionnalités décrites ci-dessous ont été ajoutées dans ce dépôt dans le cadre d'un Atelier Professionnel BTS SIO.

L'API REST utilisée par cette application est disponible ici :
**https://github.com/rabarijoy/rest_mediatekdocuments**

---

## Fonctionnalités ajoutées

### CRUD des documents

Chaque onglet (Livres, DVD, Revues) dispose désormais d'une zone de saisie permettant d'ajouter, modifier et supprimer un document.

- **Livres** : ajout avec ISBN, auteur, collection ; modification depuis la sélection ; suppression après confirmation (bloquée si des exemplaires ou des commandes existent).
- **DVD** : ajout avec durée, réalisateur, synopsis ; même logique que les livres.
- **Revues** : ajout avec périodicité et délai de mise à disposition.
- L'identifiant est calculé automatiquement (premier entier non attribué, préfixe et zéros de remplissage détectés depuis les ids existants).
- Les catégories (genre, public, rayon) sont sélectionnées via des ComboBox alimentés par l'API.

### Gestion des commandes de livres et DVD

Deux nouveaux onglets **Commandes livres** et **Commandes DVD** permettent de :

- rechercher un document par son numéro et afficher ses informations ;
- consulter la liste de ses commandes triée par date décroissante ;
- ajouter une nouvelle commande (date, montant, nombre d'exemplaires) ;
- modifier l'étape de suivi selon les règles métier suivantes :
  - une commande **Livrée** ou **Réglée** ne peut pas revenir à une étape antérieure ;
  - seule une commande **Livrée** peut passer à **Réglée** ;
- supprimer une commande, uniquement si elle est **En cours** ou **Relancée**.

### Gestion des abonnements de revues

L'onglet **Commandes revues** permet de gérer les abonnements :

- recherche d'une revue par numéro ;
- ajout d'un abonnement (date de commande, montant, date de fin) ;
- suppression d'un abonnement, bloquée si des parutions ont été reçues pendant la période de l'abonnement (vérification côté client et côté API) ;
- **alerte au démarrage** : une fenêtre modale liste les abonnements arrivant à expiration dans les 30 prochains jours (uniquement pour les profils avec accès complet).

### Gestion des exemplaires

Dans les onglets **Livres** et **DVD**, une liste d'exemplaires s'affiche automatiquement dès qu'un document est sélectionné. Dans l'onglet **Parutions des revues**, la colonne photo est remplacée par la colonne état. Pour chaque exemplaire il est possible de :

- modifier son état (sélection dans un ComboBox alimenté par l'API) ;
- le supprimer après confirmation.

### Authentification et restrictions d'accès

Un formulaire de connexion s'affiche au démarrage. Les droits accordés dépendent du service de l'utilisateur :

| Service | Accès |
|---|---|
| Administratif (00001) | Complet |
| Administrateur (00004) | Complet |
| Prêts (00002) | Consultation seule (zones de saisie et onglets commandes masqués) |
| Culture (00003) | Refusé (message d'erreur puis fermeture) |

### Sécurisation des credentials

Les paramètres de connexion à l'API sont externalisés dans `App.config` et modifiables sans recompilation :

```xml
<appSettings>
    <add key="ApiBaseUrl"  value="http://localhost/rest_mediatekdocuments/" />
    <add key="ApiUser"     value="admin" />
    <add key="ApiPassword" value="adminpwd" />
</appSettings>
```

Pour pointer vers l'API en ligne, remplacer `ApiBaseUrl` par :
```
https://apirestmediatekdocuments.medianewsonline.com/rest_mediatekdocuments/
```

### Journalisation des erreurs

Chaque erreur réseau ou retour d'erreur de l'API est enregistrée dans un fichier log journalier :

```
%AppData%\MediaTekDocuments\logs\access_yyyy-MM-dd.log
```

L'écriture est thread-safe et silencieuse en cas d'échec afin de ne pas bloquer l'application.

### Tests unitaires

Le projet `MediaTekDocuments.Tests` (MSTest, .NET Framework 4.7.2) couvre :

- `UtilitairesAbonnementTests` : 5 cas sur `ParutionDansAbonnement` (date dans la période, avant début, après fin, bornes incluses).
- `CategorieTests` : constructeur, `ToString`, héritage pour `Genre`, `Public`, `Rayon`.
- `DocumentTests` : constructeur et double héritage `LivreDvd` + `Document` pour `Livre`, `Dvd`, `Revue`.
- `ExemplaireTests` : constructeur, setters, constructeur sans paramètre (désérialisation JSON).
- `CommandeDocumentTests` : constructeur, setters, `ToString` pour `CommandeDocument` et `Suivi`.

---

## Installation locale (développement)

### Prérequis

- Windows 10 ou supérieur
- Visual Studio 2019 ou supérieur avec la charge de travail **.NET Desktop**
- .NET Framework 4.7.2
- API REST PHP en cours d'exécution (voir le dépôt `rest_mediatekdocuments`)

### Étapes

1. **Cloner le dépôt**
   ```bash
   git clone https://github.com/rabarijoy/MediaTekDocuments.git
   ```

2. **Ouvrir la solution**
   Ouvrir `MediaTekDocuments/MediaTekDocuments.sln` dans Visual Studio.

3. **Restaurer les packages NuGet**
   Dans Visual Studio : clic droit sur la solution > **Restaurer les packages NuGet**.
   Le package requis est `Newtonsoft.Json`.

4. **Configurer `App.config`**
   Vérifier ou modifier les valeurs dans `MediaTekDocuments/App.config` :
   ```xml
   <appSettings>
       <add key="ApiBaseUrl"  value="http://localhost/rest_mediatekdocuments/" />
       <add key="ApiUser"     value="admin" />
       <add key="ApiPassword" value="adminpwd" />
   </appSettings>
   ```

5. **Compiler et lancer**
   Appuyer sur `F5` ou cliquer sur **Démarrer** dans Visual Studio.

---

## Installation depuis l'installeur

Un installeur `.msi` est disponible dans le dépôt pour déployer l'application sans installer Visual Studio.

1. Télécharger le fichier zip **MediaTekSetup** depuis la section **Releases** du dépôt ou naviguer directement dans `MediaTekSetup/Release/`.
2. Extraire le contenu du zip.
3. Double-cliquer sur le fichier **`MediaTekSetup.msi`** et suivre les étapes de l'assistant d'installation.
4. Après installation, éditer le fichier `MediaTekDocuments.exe.config` situé dans le dossier d'installation pour renseigner les valeurs `ApiBaseUrl`, `ApiUser` et `ApiPassword` correspondant à l'environnement cible.

---

## Ressources

| Ressource | Emplacement |
|---|---|
| Installeur `.msi` | `MediaTekSetup/Release/MediaTekSetup.msi` |
| Documentation XML C# | `MediaTekDocuments/MediaTekDocuments.xml` (générée à la compilation) |
| Projet de tests unitaires | `MediaTekDocuments.Tests/` |
| Collection Postman (API) | `rest_mediatekdocuments/MediaTekDocuments_API_Tests.postman_collection.json` |

---

## Technologies utilisées

| Technologie | Rôle |
|---|---|
| C# .NET Framework 4.7.2 | Langage et runtime de l'application |
| Windows Forms | Interface graphique |
| `System.Net.Http.HttpClient` | Communication HTTP avec l'API REST |
| Newtonsoft.Json 13.x | Sérialisation et désérialisation JSON |
| MSTest | Tests unitaires |
| SonarCloud / SonarLint for IDE | Analyse de qualité du code |
| Git / GitHub | Versioning et gestion de projet (branches, PR, Kanban) |
