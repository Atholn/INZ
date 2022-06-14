using System.Collections.Generic;

public static class CampaignMissionsRequaried
{
    public static void SetRequired(int number)
    {
        switch (number)
        {
            case 0: MapToPlayStorage.AddSourcesRequaried(1, 200); break;
            case 1: MapToPlayStorage.AddSourcesRequaried(2, 300); break;
            case 2: MapToPlayStorage.AddBuildingRequaried(0, 1); break;
            case 3: MapToPlayStorage.AddSourcesRequaried(4, 60); break;
            case 4:
                MapToPlayStorage.AddSoldiersRequaried(1, 1);
                MapToPlayStorage.AddSoldiersRequaried(2, 1);
                MapToPlayStorage.AddSoldiersRequaried(3, 1);
                break;
            case 5: MapToPlayStorage.AddUpgradesRequaried(0, true); break;
            case 6: MapToPlayStorage.AddSoldiersRequaried(0, 20); break;
            case 7: MapToPlayStorage.AddDominateRequaried(true); break;
            case 8: MapToPlayStorage.AddSourcesRequaried(1, 3000); break;
            case 9: MapToPlayStorage.AddSourcesRequaried(2, 3000); break;
            case 10: MapToPlayStorage.AddDominateRequaried(true); break;
            case 11:
                MapToPlayStorage.AddSourcesRequaried(0, 1000);
                MapToPlayStorage.AddSourcesRequaried(3, 90);
                MapToPlayStorage.AddSourcesRequaried(4, 100);
                break;
            case 12: MapToPlayStorage.AddDominateRequaried(true); break;
            case 13: MapToPlayStorage.AddDominateRequaried(true); break;
            case 14: MapToPlayStorage.AddDominateRequaried(false); break;
            case 15: MapToPlayStorage.AddDominateRequaried(true); break;
            case 16: MapToPlayStorage.AddDominateRequaried(true); break;
            case 17: MapToPlayStorage.AddDominateRequaried(true); break;
        }
    }

    public static string ResetRequired(int number)
    {
        MapToPlayStorage.ResetRequaried();
        SetRequired(number);

        string reqText = "";
        foreach (var pair in MapToPlayStorage.WinRequarieds)
        {
            switch (pair.Key)
            {
                case "dominate": reqText += GetDominateText(pair.Value); break;
                case "sources": reqText += GetSourcesText(pair.Value); break;
                case "upgrades": reqText += GetUpgradesText(pair.Value); break;
                case "soldiers": reqText += GetSoldiersText(pair.Value); break;
                case "buldings": reqText += GetBuldingsText(pair.Value); break;
            }
            reqText += "\n";
        }
        return reqText;
    }

    private static string GetDominateText(Dictionary<string, string> reqs)
    {
        string tmpText = "";

        foreach (var pair in reqs)
        {
            switch (pair.Value)
            {
                case "win": tmpText += bool.Parse(pair.Value) ? "Dominate enemy" : "Lose with enemy"; break;
            }
        }

        return tmpText;
    }

    private static string GetSourcesText(Dictionary<string, string> reqs)
    {
        string tmpText = "Soruces requaried:\n";

        foreach (var req in reqs)
        {
            switch (req.Key)
            {
                case "0": tmpText += $"- Wood and gold total: {req.Value}"; break;
                case "1": tmpText += $"- Gold total: {req.Value}"; break;
                case "2": tmpText += $"- Wood total: {req.Value}"; break;
                case "3": tmpText += $"- Unit total: {req.Value}"; break;
                case "4": tmpText += $"- Units max total: {req.Value}"; break;
            }
            tmpText += "\n";
        }

        return tmpText;
    }

    private static string GetUpgradesText(Dictionary<string, string> reqs)
    {
        string tmpText = "Upgrades requaried:\n";

        foreach (var req in reqs)
        {
            switch (req.Key)
            {
                case "0": tmpText += bool.Parse(req.Value) ? "- A" : "- No a" + "ll upgrades"; break;
                case "1": tmpText += bool.Parse(req.Value) ? "- K" : "- No k" + "night upgrade"; break;
                case "2": tmpText += bool.Parse(req.Value) ? "- A" : "- No a" + "xeman upgrade"; break;
                case "3": tmpText += bool.Parse(req.Value) ? "- B" : "- No b" + "owman upgrade"; break;
            }
            tmpText += "\n";
        }

        return tmpText;
    }

    private static string GetSoldiersText(Dictionary<string, string> reqs)
    {
        string tmpText = "Soldiers requaried:\n";

        foreach (var req in reqs)
        {
            switch (req.Key)
            {
                case "0": tmpText += $"- All soldiers count: {req.Value}"; break;
                case "1": tmpText += $"- Knights count: {req.Value}"; break;
                case "2": tmpText += $"- Axemans count: {req.Value}"; break;
                case "3": tmpText += $"- Bowmans count: {req.Value}"; break;
            }
            tmpText += "\n";
        }

        return tmpText;
    }

    private static string GetBuldingsText(Dictionary<string, string> reqs)
    {
        string tmpText = "Soldiers requaried:\n";

        foreach (var req in reqs)
        {
            switch (req.Key)
            {
                case "0": tmpText += $"- Each building in the amount of: {req.Value}"; break;
                case "1": tmpText += $"- Townhalls count: {req.Value}"; break;
                case "2": tmpText += $"- Barracks count: {req.Value}"; break;
                case "3": tmpText += $"- Farms count: {req.Value}"; break;
                case "4": tmpText += $"- Blacksmiths count: {req.Value}"; break;
                case "5": tmpText += $"- Towers count: {req.Value}"; break;
            }
            tmpText += "\n";
        }

        return tmpText;
    }

}
