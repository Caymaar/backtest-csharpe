using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class Database
{
    // Propriétés pour stocker le chemin du fichier et les données lues
    private readonly string _filePath;
    private readonly Dictionary<DateTime, Dictionary<string, double>> _data;

    // Constructeur qui prend en paramètre le chemin du fichier CSV et initialise les données
    public Database(string filePath)
    {
        _filePath = filePath; // Chemin du fichier CSV
        _data = new Dictionary<DateTime, Dictionary<string, double>>(); // Dictionnaire pour stocker les données
        LoadData(); // Chargement des données depuis le fichier CSV
    }

    // Méthode pour charger les données du fichier CSV
    private void LoadData()
    {
        // Ouverture du fichier en lecture avec StreamReader
        using (var reader = new StreamReader(_filePath))
        {
            // Lecture de la première ligne (en-tête)
            string headerLine = reader.ReadLine();
            if (headerLine == null) throw new Exception("Le fichier CSV est vide."); // Vérification de la présence d'une en-tête

            // Séparation des en-têtes par des virgules
            var headers = headerLine.Split(',');
            if (headers.Length < 2) throw new Exception("Le fichier CSV doit avoir au moins deux colonnes."); // Vérification qu'il y a bien des colonnes de données

            // Boucle sur chaque ligne de données jusqu'à la fin du fichier
            while (!reader.EndOfStream)
            {
                // Lecture d'une ligne de données
                string line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue; // Ignorer les lignes vides

                // Séparation des valeurs par des virgules
                var values = line.Split(',');
                if (values.Length != headers.Length) continue; // Vérification de la cohérence des colonnes

                // Conversion de la première colonne en type DateTime
                if (!DateTime.TryParseExact(values[0], "yyyy-MM-dd HH:mm:ssK", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    Console.WriteLine($"Format de date invalide pour la ligne: {line}"); // Avertissement en cas de format de date invalide
                    continue; // Ignorer les lignes au format invalide
                }

                // Dictionnaire pour stocker les valeurs de chaque ticker pour la date actuelle
                var rowData = new Dictionary<string, double>();
                for (int i = 1; i < values.Length; i++)
                {
                    // Conversion de la valeur en type double
                    if (double.TryParse(values[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                    {
                        rowData[headers[i]] = price; // Stockage de la valeur dans le dictionnaire rowData avec le ticker en clé
                    }
                    else
                    {
                        // Avertissement en cas de valeur non numérique pour un ticker
                        Console.WriteLine($"Valeur non numérique trouvée pour la colonne {headers[i]} à la date {date}");
                    }
                }

                // Ajout de la date et des données de ticker correspondantes dans le dictionnaire principal _data
                _data[date] = rowData;
            }
        }
    }

    // Méthode pour récupérer les données filtrées par date de début et de fin
    public Dictionary<DateTime, Dictionary<string, double>> GetData(DateTime startDate, DateTime endDate)
    {
        // Dictionnaire pour stocker les résultats filtrés
        var result = new Dictionary<DateTime, Dictionary<string, double>>();

        // Parcours des données pour filtrer par date
        foreach (var entry in _data)
        {
            if (entry.Key >= startDate && entry.Key <= endDate) // Si la date est dans l'intervalle
            {
                result[entry.Key] = entry.Value; // Ajout des données dans le résultat
            }
        }

        return result; // Retourne les données filtrées
    }

    // Méthode pour afficher les données filtrées dans la console (utile pour le débogage)
    public void PrintData(Dictionary<DateTime, Dictionary<string, double>> data)
    {
        // Parcours de chaque entrée de date dans les données
        foreach (var dateEntry in data)
        {
            Console.Write($"{dateEntry.Key:yyyy-MM-dd HH:mm:ssK} "); // Affiche la date

            // Parcours de chaque ticker pour la date
            foreach (var ticker in dateEntry.Value)
            {
                Console.Write($"{ticker.Key}: {ticker.Value} "); // Affiche le ticker et son prix
            }
            Console.WriteLine(); // Nouvelle ligne pour chaque date
        }
    }
}


class program
{
    static void Main()
    {
        // Initialisation du lecteur CSV avec le chemin du fichier
        var csvReader = new Database("C:\\Users\\admin\\Desktop\\cours dauphine\\C#\\Projet\\Database\\Univers.csv");

        // Définition de la période souhaitée avec la date de début et de fin
        DateTime startDate = new DateTime(2023, 1, 3);
        DateTime endDate = new DateTime(2023, 1, 5);

        // Récupération des données filtrées pour la période spécifiée
        var filteredData = csvReader.GetData(startDate, endDate);

        // Affichage des données filtrées dans la console
        Console.WriteLine("Données filtrées entre le {0} et le {1}:", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
        csvReader.PrintData(filteredData); // Appel de la méthode PrintData pour afficher les données

        Console.ReadLine();
    }
}
