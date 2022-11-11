using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class DB : MonoBehaviour {

    

    #region Conexion

    public string rutaDB;
    public string conexion;
    public IDbConnection dbConnection;
    public IDbCommand dbCommand;
    public IDataReader dataReader;
    public string nombreDB = "ComidasDB.db";

    public void AbrirDB(){
        rutaDB = Application.dataPath + "/StreamingAssets/" + nombreDB;
        conexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(conexion);
        dbConnection.Open();
    }

    public void CerrarDB(){
        dataReader.Close();
        dataReader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }
    #endregion Conexion

    private void Start(){
        AbrirDB();
        InvocarComidasIniciales();
    }

    #region ComidasIniciales

    public GameObject comidasContainer;
    public GameObject prefabComida;

    public void InvocarComidasIniciales(){

        LimpiarComidas();

        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT * FROM Comidas";
        dbCommand.CommandText = sqlQuery;
        dataReader = dbCommand.ExecuteReader();

        while(dataReader.Read()){

            // int id = dataReader.GetInt32(0);
            // string dlc = dataReader.GetString(2);
            // string coccion = dataReader.GetString(7);

            GameObject prefab = Instantiate(prefabComida);
            prefab.transform.SetParent(comidasContainer.gameObject.transform, false);
            prefab.gameObject.name = "Prefab: " + dataReader.GetString(1);

            prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(dataReader.GetString(1)); //Icono
            prefab.transform.GetChild(1).GetComponent<Text>().text = dataReader.GetString(1); //Nombre
            prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dataReader.GetFloat(3).ToString(); //Stat: Vida
            prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(4).ToString(); //Stat: Hambre
            prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = dataReader.GetFloat(5).ToString(); //Stat: Cordura
            prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = dataReader.GetFloat(6).ToString(); //Stat: Putrefaccion

        }
    }

    #endregion

    #region BusquedaDeComida
    public InputField busquedaIF;

    public void BuscarComida(){

        LimpiarComidas();

        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("SELECT * FROM Comidas WHERE Nombre = \"{0}\"", busquedaIF.text);  
        dbCommand.CommandText = sqlQuery;
        dataReader = dbCommand.ExecuteReader();

        if(dataReader.Read()){

            GameObject prefab = Instantiate(prefabComida);
            prefab.transform.SetParent(comidasContainer.gameObject.transform, false);
            prefab.gameObject.name = "Prefab: " + dataReader.GetString(1);

            prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(dataReader.GetString(1)); //Icono
            prefab.transform.GetChild(1).GetComponent<Text>().text = dataReader.GetString(1); //Nombre
            prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dataReader.GetString(3); //Stat: Vida
            prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = dataReader.GetString(4); //Stat: Hambre
            prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = dataReader.GetString(5); //Stat: Cordura
            prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = dataReader.GetString(6); //Stat: Putrefaccion

        }else if(busquedaIF.text == "" || busquedaIF.text == null){
            InvocarComidasIniciales();
        }else{
            Debug.Log("No hay comidas por mostrar!");
        }
    }

    private void LimpiarComidas(){

        foreach(Transform t in  comidasContainer.transform){
            Destroy(t.gameObject);
        }

    }
    #endregion

    #region OrdenarComidas

    public void OrdenarPorEstadistica(string statComida){

        LimpiarComidas();

        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("SELECT * FROM Comidas ORDER BY \"{0}\" DESC", statComida);    
        dbCommand.CommandText = sqlQuery;
        dataReader = dbCommand.ExecuteReader();

        while(dataReader.Read()){

            GameObject prefab = Instantiate(prefabComida);
            prefab.transform.SetParent(comidasContainer.gameObject.transform, false);
            prefab.gameObject.name = "Prefab: " + dataReader.GetString(1);

            prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(dataReader.GetString(1)); //Icono
            prefab.transform.GetChild(1).GetComponent<Text>().text = dataReader.GetString(1); //Nombre
            prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dataReader.GetFloat(3).ToString(); //Stat: Vida
            prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(4).ToString(); //Stat: Hambre
            prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = dataReader.GetFloat(5).ToString(); //Stat: Cordura
            prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = dataReader.GetFloat(6).ToString(); //Stat: Putrefaccion

        }
    }


    #endregion
    
}
