namespace dbman
{
    using SQLite4Unity3d;
    using UnityEngine;
#if !UNITY_EDITOR
    using System.Collections;
    using System.IO;
#endif
    using System.Collections.Generic;
    using System;

    public class dbManager
    {
        private SQLiteConnection _connection;

        public dbManager(string DatabaseName)
        {
#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
            // check if file exists in Application.persistentDataPath
            var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            Debug.Log("Final PATH: " + dbPath);

        }


        public Emotions CreateEmotion(string emote, float score, DateTime time, string hobj, string obj)
        {

            var e = new Emotions
            {
                Emotion = emote,
                Score = score,
                Time = time,
                HighestObject = hobj,
                Object = obj
            };
            _connection.Insert(e);
            Debug.Log("EarlDebug dbMan: insert succes");
            return e;
        }

        public IEnumerable<Emotions> getEmotions()
        {
            return _connection.Table<Emotions>();
        }

        public void ePrintData(IEnumerable<Emotions> emotes)
        {
            int i = 0;
            foreach (var data in emotes)
            {
                Debug.Log("EarlDebug dbManGet: " + i + " Data: " + data.ToString());
                i++;
            }
        }

        public void createTable()
        {
            _connection.CreateTable<Emotions>();
        }
    }
}

