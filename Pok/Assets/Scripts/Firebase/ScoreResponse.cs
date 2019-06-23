using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class ScoreResponse
{
    public List<ScoreEntryData> entries;
    public int senderIndex;
}