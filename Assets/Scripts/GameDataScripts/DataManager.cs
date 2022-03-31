using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
   public static DataManager instance;

   public string scoreData = "/ScoreData.dat";

   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
   }

   public int GetScore()
   {
      string dataPath = Application.persistentDataPath + scoreData;
      int hightScore = 0;

      if (File.Exists(dataPath))
      {
         BinaryFormatter formatter = new BinaryFormatter();
         FileStream stream = new FileStream(dataPath, FileMode.Open);

         GameData gameData = formatter.Deserialize(stream)as GameData;

         hightScore = gameData.highScore;
         stream.Close();
      }
      else
      {
         SetScore(0);
      }
      
      return hightScore;
   }

   public void SetScore(int score)
   {
      BinaryFormatter formatter = new BinaryFormatter();
      string dataPath = Application.persistentDataPath + scoreData;

      FileStream stream = new FileStream(dataPath, FileMode.Create);

      GameData gameData = new GameData();
      gameData.highScore = score;
      
      formatter.Serialize(stream,gameData);
      stream.Close();
   }
   
}
