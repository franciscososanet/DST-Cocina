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

            GameObject prefab = Instantiate(prefabComida);
            prefab.transform.SetParent(comidasContainer.gameObject.transform, false);
            prefab.gameObject.name = dataReader.GetString(1);
            
            prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(dataReader.GetString(1)); //Icono
            prefab.transform.GetChild(1).GetComponent<Text>().text = dataReader.GetString(1); //Nombre
            prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dataReader.GetFloat(3).ToString(); //Stat: Vida
            prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(4).ToString(); //Stat: Hambre
            prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = dataReader.GetFloat(5).ToString(); //Stat: Cordura
            prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = dataReader.GetFloat(6).ToString(); //Stat: Putrefaccion
            prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = dataReader.GetString(2); //Invisible: DLC 
            prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(7).ToString(); //Invisible: Coccion 
            prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = dataReader.GetString(8); //Invisible: Ingredientes

            string nombre = prefab.transform.GetChild(1).GetComponent<Text>().text;
            string dlc = prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text;
            string vida = prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text;
            string hambre = prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text;
            string cordura = prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text;
            string putrefaccion = prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text;
            string coccion = prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text;
            string ingredientes = prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text;
            
            prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {ObtenerInfo(nombre, dlc, vida, hambre, cordura, putrefaccion, coccion, ingredientes); });

        }
    }

    #endregion

    #region BusquedaDeComida
    public InputField busquedaIF;

    public void BuscarComida(){

        LimpiarComidas();

        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("SELECT * FROM Comidas WHERE Nombre LIKE '%{0}%' OR Equivalencias LIKE '%{0}%' OR Requisitos LIKE '%{0}%' --case-insensitive", busquedaIF.text);
        dbCommand.CommandText = sqlQuery;
        dataReader = dbCommand.ExecuteReader();

        try{
            while(dataReader.Read()){

                GameObject prefab = Instantiate(prefabComida);
                prefab.transform.SetParent(comidasContainer.gameObject.transform, false);

                prefab.gameObject.name = dataReader.GetString(1);
                prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(dataReader.GetString(1)); //Icono
                prefab.transform.GetChild(1).GetComponent<Text>().text = dataReader.GetString(1); //Nombre
                prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = dataReader.GetString(2);//DLC
                prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dataReader.GetFloat(3).ToString(); //Stat: Vida
                prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(4).ToString(); //Stat: Hambre
                prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = dataReader.GetFloat(5).ToString(); //Stat: Cordura
                prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = dataReader.GetFloat(6).ToString(); //Stat: Putrefaccion
                prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(7).ToString(); //Stat: Coccion
                prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = dataReader.GetString(8); //Ingredientes

                string nombre = prefab.transform.GetChild(1).GetComponent<Text>().text;
                string dlc = prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text;
                string vida = prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text;
                string hambre = prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text;
                string cordura = prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text;
                string putrefaccion = prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text;
                string coccion = prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text;
                string ingredientes = prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text;

                prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {ObtenerInfo(nombre, dlc, vida, hambre, cordura, putrefaccion, coccion, ingredientes); });
            }    
        }catch(Exception e){
            Debug.Log("ERROR: " + e);
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
        string sqlQuery = String.Format("SELECT * FROM Comidas WHERE Nombre LIKE '%{0}%' OR Equivalencias LIKE '%{0}%' OR Requisitos LIKE '%{0}%' ORDER BY \"{1}\" DESC --case-insensitive", busquedaIF.text, statComida);    
        dbCommand.CommandText = sqlQuery;
        dataReader = dbCommand.ExecuteReader();

        while(dataReader.Read()){

            GameObject prefab = Instantiate(prefabComida);
            prefab.transform.SetParent(comidasContainer.gameObject.transform, false);
            prefab.gameObject.name = dataReader.GetString(1);
            
            prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(dataReader.GetString(1)); //Icono
            prefab.transform.GetChild(1).GetComponent<Text>().text = dataReader.GetString(1); //Nombre
            prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dataReader.GetFloat(3).ToString(); //Stat: Vida
            prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(4).ToString(); //Stat: Hambre
            prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = dataReader.GetFloat(5).ToString(); //Stat: Cordura
            prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = dataReader.GetFloat(6).ToString(); //Stat: Putrefaccion
            prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = dataReader.GetString(2); //Invisible: DLC 
            prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(7).ToString(); //Invisible: Coccion 
            prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = dataReader.GetString(8); //Invisible: Ingredientes

            string nombre = prefab.transform.GetChild(1).GetComponent<Text>().text;
            string dlc = prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text;
            string vida = prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text;
            string hambre = prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text;
            string cordura = prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text;
            string putrefaccion = prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text;
            string coccion = prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text;
            string ingredientes = prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text;
            
            prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {ObtenerInfo(nombre, dlc, vida, hambre, cordura, putrefaccion, coccion, ingredientes); });

        }
    }

    #endregion
    
    #region MostrarInfoComida

    public GameObject panelInfo;
    public GameObject ingredientesContainer;
    public GameObject prefabIngredienteTxt;

    private void LimpiarIngredientes(){
        foreach(Transform t in  ingredientesContainer.transform){
            Destroy(t.gameObject);
        }
    }

    public void OcultarPanelInfo(){
        panelInfo.SetActive(false);
    }

    public void ObtenerInfo(string nombre, string dlc, string vida, string hambre, string cordura, string putrefaccion, string coccion, string ingredientes){

        LimpiarIngredientes();

        switch(dlc){
            case "Shipwrecked":
            panelInfo.transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Shipwrecked"); 
            panelInfo.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
            break;

            case "Reign of Giants":
            panelInfo.transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Reign of Giants");
            panelInfo.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
            break;

            default:
            panelInfo.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            break;
        }

        panelInfo.transform.GetChild(1).GetComponent<Text>().text = nombre;
        panelInfo.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dlc;
        panelInfo.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = coccion + " segundos";

        string[] ingredientesArray = ingredientes.Split(", ");

        foreach(string ingrediente in ingredientesArray){

            GameObject prefab = Instantiate(prefabIngredienteTxt);
            prefab.transform.SetParent(ingredientesContainer.gameObject.transform, false);
            prefab.gameObject.name = ingrediente;

            prefab.GetComponent<Text>().text = ingrediente;
            
            string ingredienteTrimmeado = ingrediente.Trim( new Char[] {' ', '.', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'});

            switch(nombre){
                case "Pegote húmedo":
                prefab.transform.GetChild(0).gameObject.SetActive(false);
                break;

                case "Pavo asado": //TODO: Es un ingrediente U otro. Arreglar
                prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ingredienteTrimmeado);
                break;

                default:
                prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ingredienteTrimmeado);
                break;
            }

            prefab.SetActive(true);
        }

        panelInfo.SetActive(true);

    }
    
    #endregion
}
