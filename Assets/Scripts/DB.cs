using System;
using System.Collections;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DB : MonoBehaviour {

    #region Conexion

    public string rutaDB;
    public string conexion;
    public IDbConnection dbConnection;
    public IDbCommand dbCommand;
    public IDataReader dataReader;
    public string nombreDB = "ComidasDB.db";

    public void CerrarDB(){
        dataReader.Close();
        dataReader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    IEnumerator AbrirDBAndroid(){
        UnityWebRequest uwr = new UnityWebRequest("jar:file://" + Application.dataPath + "!/assets/" + nombreDB);
        uwr.downloadHandler = new DownloadHandlerBuffer();
            
        yield return uwr.SendWebRequest();

        File.WriteAllBytes(rutaDB, uwr.downloadHandler.data);
        InvocarComidasIniciales();
    }

    #endregion Conexion

    private void Start(){

        if(Application.platform == RuntimePlatform.WindowsEditor){
            rutaDB = Application.dataPath + "/StreamingAssets/" + nombreDB;
        }

        if(Application.platform == RuntimePlatform.Android){
            rutaDB = Application.persistentDataPath + "/" + nombreDB;
            if(!File.Exists(rutaDB)){
                StartCoroutine(AbrirDBAndroid());
            } 
        }

        conexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(conexion);
        dbConnection.Open();

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
            prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = dataReader.GetString(3); //Invisible: DLC 
            prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dataReader.GetFloat(4).ToString(); //Stat: Vida
            prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(5).ToString(); //Stat: Hambre
            prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = dataReader.GetFloat(6).ToString(); //Stat: Cordura
            prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = dataReader.GetFloat(7).ToString(); //Stat: Putrefaccion
            prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(8).ToString(); //Invisible: Coccion 
            prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = dataReader.GetString(9); //Invisible: Requisitos
            prefab.transform.GetChild(3).GetChild(3).GetComponent<Text>().text = dataReader.GetString(10); //Invisible: Restricciones

            string nombre = prefab.transform.GetChild(1).GetComponent<Text>().text;
            string dlc = prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text;
            string vida = prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text;
            string hambre = prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text;
            string cordura = prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text;
            string putrefaccion = prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text;
            string coccion = prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text;
            string requisitos = prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text;
            string restricciones = prefab.transform.GetChild(3).GetChild(3).GetComponent<Text>().text;
            
            prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {ObtenerInfo(nombre, dlc, vida, hambre, cordura, putrefaccion, coccion, requisitos, restricciones); }); //agregar restricicones

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
                prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = dataReader.GetString(8); //Requisitos

                string nombre = prefab.transform.GetChild(1).GetComponent<Text>().text;
                string dlc = prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text;
                string vida = prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text;
                string hambre = prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text;
                string cordura = prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text;
                string putrefaccion = prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text;
                string coccion = prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text;
                string requisitos = prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text;
                string restricciones = "a";

                prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {ObtenerInfo(nombre, dlc, vida, hambre, cordura, putrefaccion, coccion, requisitos, restricciones); });
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
            prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = dataReader.GetString(8); //Invisible: Requisitos

            string nombre = prefab.transform.GetChild(1).GetComponent<Text>().text;
            string dlc = prefab.transform.GetChild(3).GetChild(0).GetComponent<Text>().text;
            string vida = prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text;
            string hambre = prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text;
            string cordura = prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text;
            string putrefaccion = prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text;
            string coccion = prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text;
            string requisitos = prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text;
            string restricciones = "b";
            
            prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {ObtenerInfo(nombre, dlc, vida, hambre, cordura, putrefaccion, coccion, requisitos, restricciones); });

        }
    }

    #endregion
    
    #region MostrarInfoComida

    public GameObject panelInfo;
    public GameObject requisitosContainer;
    public GameObject restriccionesContainer;
    public GameObject prefabIngredienteTxt;
    public GameObject prefabRestriccionTxt;
    public Text tipoRestriccion;

    private void LimpiarRequisitos(){
        foreach(Transform t in requisitosContainer.transform){
            Destroy(t.gameObject);
        }
    }

    private void LimpiarRestricciones(){
        foreach(Transform t in restriccionesContainer.transform){
            Destroy(t.gameObject);
        }
    }

    public void OcultarPanelInfo(){
        panelInfo.SetActive(false);
    }

    public void ObtenerInfo(string nombre, string dlc, string vida, string hambre, string cordura, string putrefaccion, string coccion, string requisitos, string restricciones){

        LimpiarRequisitos();
        LimpiarRestricciones();

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

        string[] requisitosArray = requisitos.Split(", ");

        foreach(string requisito in requisitosArray){

            GameObject prefabI = Instantiate(prefabIngredienteTxt);
            prefabI.transform.SetParent(requisitosContainer.gameObject.transform, false);
            prefabI.gameObject.name = requisito;

            prefabI.GetComponent<Text>().text = requisito;
            
            string ingredienteTrimmeado = requisito.Trim( new Char[] {' ', '.', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'});

            switch(nombre){
                case "Pegote húmedo":
                prefabI.transform.GetChild(0).gameObject.SetActive(false);
                break;

                case "Pavo asado": //TODO: Es un ingrediente U otro. Arreglar
                prefabI.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ingredienteTrimmeado);
                break;

                default:
                prefabI.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ingredienteTrimmeado);
                break;
            }

            prefabI.SetActive(true);
        }

        string[] restriccionesArray = restricciones.Split(": ");

        if(1 < restriccionesArray.Length){
            
            string[] restriccionesSeparadas = restriccionesArray[1].Split(", ");

            foreach(string restriccion in restriccionesSeparadas){

                GameObject prefabR = Instantiate(prefabRestriccionTxt);        
                prefabR.transform.SetParent(restriccionesContainer.gameObject.transform, false);

                prefabR.gameObject.name = restriccionesArray[0] + " " + restriccionesArray[1];
                prefabR.GetComponent<Text>().text = restriccion;
                prefabR.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(restriccion);

                Debug.Log(restriccion);
            }
        
        }
        
        switch(restriccionesArray[0]){

            case "NO":
            tipoRestriccion.text = "NO PONER";
            break;

            case "COCINANDO EN":
            tipoRestriccion.text = "COCINAR EN";
            break;

            case "MÁXIMO":
            tipoRestriccion.text = "MÁXIMO";
            break;

            case "SOLO":
            tipoRestriccion.text = "SOLO PONER";
            break;

            default:
            tipoRestriccion.text = "";
            break;
        }

        panelInfo.SetActive(true);

    }
    
    #endregion
}
