using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sqlite libraries
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class DBmanager : MonoBehaviour
{
	//Game Mode
	[HideInInspector]
	public bool arcadeMode;
	
    //Variables de inicialización de la base de datos y abrirla
    string routeDB;
    string strConection;
    string DBfileName = "RecordsDatabase.db";

    //Para trabajar con las conexiones
    private IDbConnection _dbConnection;
    //Para ejecutar comandos
    private IDbCommand _dbCommand;
    //Para leer la base de datos
    private IDataReader _reader;
    
    //Canvas
    [HideInInspector]
    public GameObject recordFieldPrefab;


    // Use this for initialization
	public void ReadTopDb()
	{
		Debug.Log("DB OPENING...");
		OpenDB();
		if (arcadeMode)
		{
			ReadTopRecordsDB("ArcadeRecords");
		}
		else
		{
			ReadTopRecordsDB("StoryRecords");
		}
		CloseDB();
	}

	public bool TestBeatedRecordsDB(int actualScore)
	{
		OpenDB();
		if (arcadeMode)
		{
			return BeatRecordTestDB("ArcadeRecords", actualScore);
		}
		else
		{
			return BeatRecordTestDB("StoryRecords", actualScore);
		}
		CloseDB();
	}

	public void UpdateIDs()
	{
		if (arcadeMode)
		{
			List<int> IDsForUpdate = GetIdListDB("ArcadeRecords");
			OpenDB();
			for (int i = 0; i < IDsForUpdate.Count; i++)
			{
				UpdateRecordTableIDs("ArcadeRecords", i, IDsForUpdate[i]);
			}
			_reader.Close();
			_dbConnection.Close();
			_reader = null;
			_dbConnection = null;
		}
		else
		{
			List<int> IDsForUpdate = GetIdListDB("StoryRecords");
			OpenDB();
			for (int i = 0; i < IDsForUpdate.Count; i++)
			{
				UpdateRecordTableIDs("StoryRecords", i, IDsForUpdate[i]);
			}
			_reader.Close();
			_dbConnection.Close();
			_reader = null;
			_dbConnection = null;
		}
	}
	
	public void ReplaceScoresDB(string nameNewPlayer, int scoreNewPlayer)
	{ 
		if (LimitTestTableReachDB()) 
		{
		 	Debug.Log("limit over 10");
		 	DeleteLastRecordFromDB();
		}
		AddNewRecordToDB(nameNewPlayer, scoreNewPlayer);
	}

	public void AddNewRecordToDB(string name, int score)
	{
		OpenDB();
		if (arcadeMode)
		{
			AddNewRecordDB("ArcadeRecords", name, score);
		}
		else
		{
			AddNewRecordDB("StoryRecords", name, score);
		}
		CloseDB();
	}

	public void DeleteLastRecordFromDB()
	{
		OpenDB();
		if (arcadeMode)
		{
			DeleteLastRecordDB("ArcadeRecords");
		}
		else
		{
			DeleteLastRecordDB("StoryRecords");
		}
		CloseDB();
	}

	public bool LimitTestTableReachDB()
	{
		OpenDB();
		if (arcadeMode)
		{
			return LimitTableReachedDB("ArcadeRecords");
		}
		else
		{
			return LimitTableReachedDB("StoryRecords");
		}
	}

	public int GetMaxScoreDB(int actualScore)
	{
		OpenDB();
		if (arcadeMode)
		{
			return FindMaxScoreDB("ArcadeRecords", actualScore);
		}
		else
		{
			return FindMaxScoreDB("StoryRecords", actualScore);
		}
	}

	public int GetHighScoreDB(string table)
	{
		OpenDB();
		return FindHighScoreDB(table);
	}

	
	//Método para abrir la base de datos
	void OpenDB()
	{
		//Crear y abrir la conexión
		routeDB = Application.dataPath + "/StreamingAssets/" + DBfileName;

		strConection = "URI=file:" + routeDB;
		_dbConnection = new SqliteConnection(strConection); 
		_dbConnection.Open();
	}

	//Metodos de utilidad
	void ReadTopRecordsDB(string table)
	{
		//Crear la consulta
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"select * from {table} order by Points desc limit 10";
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();

		int posPlayer = 0;
		
		Debug.Log("Reading...");
		
		while (_reader.Read())
		{
			try
			{
				int playerID = _reader.GetInt32(0);
				string playerName = _reader.GetString(1);
				int playerPoints = _reader.GetInt32(2);
				posPlayer++;
				GameObject recordFieldObj = Instantiate(recordFieldPrefab, this.transform);
				if(posPlayer < 10) recordFieldObj.GetComponent<TextMeshProUGUI>().text = $"#{posPlayer}  -  {playerName}  -  {playerPoints}";
				else recordFieldObj.GetComponent<TextMeshProUGUI>().text = $"#{posPlayer} -  {playerName}  -  {playerPoints}";
			}
			catch (FormatException formatError)
			{
				Debug.Log(formatError.Message);
				continue;
			}
			catch (Exception error)
			{
				Debug.Log(error.Message);
				continue;
			}
		}
	}

	void DeleteLastRecordDB(string table)
	{
		//Crear la consulta
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"delete from {table} where id = (select max(id) from {table} where points = (select min(points) from {table}))";
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();
		
		Debug.Log("Deleted last player");
	}

	void UpdateRecordTableIDs(string table, int idReset, int idForCondition)
	{
		//Crear la consulta
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"update {table} set id = {idReset} where id = {idForCondition}";
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();

		_dbCommand.Dispose();
		_dbCommand = null;
	}

	List<int> GetIdListDB(string table)
	{
		OpenDB();
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"select id from {table} order by id asc";
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();

		List<int> IDsFromTable = new List<int>();
		while (_reader.Read())
		{
			try
			{
				IDsFromTable.Add(_reader.GetInt32(0));
			}
			catch (FormatException formatError)
			{
				Debug.Log(formatError.Message);
				continue;
			}
			catch (Exception error)
			{
				Debug.Log(error.Message);
				continue;
			}
		}

		CloseDB();
		return IDsFromTable;
	}

	int FindMaxScoreDB(string table, int actualScore)
	{
		int ms = 0;
		//Crear la consulta
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"select Points from {table} order by Points asc";
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();

		while (_reader.Read())
		{
			try
			{
				int playerPoints = _reader.GetInt32(0);
				if (actualScore < playerPoints)
				{
					ms = playerPoints;
					break;
				}
			}
			catch (FormatException formatError)
			{
				Debug.Log(formatError.Message);
				continue;
			}
			catch (Exception error)
			{
				Debug.Log(error.Message);
				continue;
			}
		}

		CloseDB();
		return ms;
	}

	int FindHighScoreDB(string table)
	{
		int hs = 0;
		//Crear la consulta
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"select Points from {table} order by Points desc limit 1";
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();

		while (_reader.Read())
		{
			try
			{
				hs = _reader.GetInt32(0);
			}
			catch (FormatException formatError)
			{
				Debug.Log(formatError.Message);
				continue;
			}
			catch (Exception error)
			{
				Debug.Log(error.Message);
				continue;
			}
		}

		CloseDB();
		return hs;
	}

	bool BeatRecordTestDB(string table, int actualScore)
	{
		bool beated = false;
		//Crear la consulta
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"select Points from {table} order by Points asc";
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();

		while (_reader.Read())
		{
			try
			{
				int playerPoints = _reader.GetInt32(0);
				if (actualScore > playerPoints)
				{
					beated = true;
					break;
				}
			}
			catch (FormatException formatError)
			{
				Debug.Log(formatError.Message);
				continue;
			}
			catch (Exception error)
			{
				Debug.Log(error.Message);
				continue;
			}
		}

		
		CloseDB();
		return beated;
	}

	bool LimitTableReachedDB(string table)
	{
		bool limited = false;
		//Crear la consulta
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"select count(ID) from {table}";
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();

		while (_reader.Read())
		{
			try
			{
				int numberRows = _reader.GetInt32(0);
				if (numberRows >= 10)
				{
					limited = true;
					break;
				}
			}
			catch (FormatException formatError)
			{
				Debug.Log(formatError.Message);
				continue;
			}
			catch (Exception error)
			{
				Debug.Log(error.Message);
				continue;
			}
		}

		Debug.Log("Test the limit");
		CloseDB();
		return limited;
	}
	
	void AddNewRecordDB(string table, string name, int score)
	{
		//Crear la consulta
		_dbCommand = _dbConnection.CreateCommand();
		string sqlQuery = $"insert into {table}(Name, Points) values('{name.ToUpper()}',{score})";
		Debug.Log(sqlQuery);
		_dbCommand.CommandText = sqlQuery;
		//Leer la base de datos
		_reader = _dbCommand.ExecuteReader();
		
		Debug.Log("inserted new player with score: " + score);
	}

	void CloseDB()
	{
		//Cerrar las conexiones
		_reader.Close();
		_reader = null;
		_dbCommand.Dispose();
		_dbCommand = null;
		_dbConnection.Close();
		_dbConnection = null;
	}


}

