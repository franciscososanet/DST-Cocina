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
        InvocarComidas();
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

        InvocarComidas();
    }

    #region ComidasIniciales

    public GameObject comidasContainer;
    public GameObject prefabComida;

    public void InvocarComidas(string QUERY = "SELECT * FROM Comidas"){

        LimpiarComidas();

        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("{0}", QUERY);
        
        dbCommand.CommandText = sqlQuery;
        dataReader = dbCommand.ExecuteReader();

        while(dataReader.Read()){

            GameObject prefab = Instantiate(prefabComida);
            prefab.transform.SetParent(comidasContainer.gameObject.transform, false);
            prefab.gameObject.name = dataReader.GetString(1);
            
            prefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(dataReader.GetString(1)); //Icono
            prefab.transform.GetChild(1).GetComponent<Text>().text = dataReader.GetString(1); //Nombre
            prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = dataReader.GetFloat(4).ToString(); //Stat: Vida
            prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = dataReader.GetFloat(5).ToString(); //Stat: Hambre
            prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text = dataReader.GetFloat(6).ToString(); //Stat: Cordura
            prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = dataReader.GetFloat(7).ToString(); //Stat: Putrefaccion
            prefab.transform.GetChild(2).GetChild(4).GetComponent<Text>().text = dataReader.GetFloat(8).ToString() + "''"; //Stat: Coccion 
            prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text = dataReader.GetString(9); //Invisible: Requisitos
            prefab.transform.GetChild(3).GetChild(3).GetComponent<Text>().text = dataReader.GetString(10); //Invisible: Restricciones

            string nombre = prefab.transform.GetChild(1).GetComponent<Text>().text;
            string vida = prefab.transform.GetChild(2).GetChild(0).GetComponent<Text>().text;
            string hambre = prefab.transform.GetChild(2).GetChild(1).GetComponent<Text>().text;
            string cordura = prefab.transform.GetChild(2).GetChild(2).GetComponent<Text>().text;
            string putrefaccion = prefab.transform.GetChild(2).GetChild(3).GetComponent<Text>().text;
            string coccion = prefab.transform.GetChild(3).GetChild(1).GetComponent<Text>().text;
            string requisitos = prefab.transform.GetChild(3).GetChild(2).GetComponent<Text>().text;
            string restricciones = prefab.transform.GetChild(3).GetChild(3).GetComponent<Text>().text;
            
            
            prefab.GetComponent<Button>().onClick.AddListener(delegate {ObtenerInfo(nombre, vida, hambre, cordura, putrefaccion, coccion, requisitos, restricciones); }); 
            prefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {ObtenerInfo(nombre, vida, hambre, cordura, putrefaccion, coccion, requisitos, restricciones); });
        }
    }

    private void LimpiarComidas(){
        foreach(Transform t in  comidasContainer.transform){
            Destroy(t.gameObject);
        }
    }

    #endregion

    #region Querys

    public InputField busquedaIF;

    public void BuscarComida(){
        InvocarComidas(String.Format("SELECT * FROM Comidas WHERE Nombre LIKE '%{0}%' OR Equivalencias LIKE '%{0}%' OR Requisitos LIKE '%{0}%' --case-insensitive", busquedaIF.text));        
    }

    public void OrdenarPorEstadistica(string statComida){
        InvocarComidas(String.Format("SELECT * FROM Comidas WHERE Nombre LIKE '%{0}%' OR Equivalencias LIKE '%{0}%' OR Requisitos LIKE '%{0}%' ORDER BY \"{1}\" DESC --case-insensitive", busquedaIF.text, statComida));
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
        LimpiarRequisitos();
        LimpiarRestricciones();
        panelInfo.SetActive(false);
    }

    public void ObtenerInfo(string nombre, string vida, string hambre, string cordura, string putrefaccion, string coccion, string requisitos, string restricciones){

        LimpiarRequisitos();
        LimpiarRestricciones();

        panelInfo.transform.GetChild(1).GetComponent<Text>().text = nombre;

        string[] requisitosArray = requisitos.Split(", ");

        foreach(string requisito in requisitosArray){

            GameObject prefabI = Instantiate(prefabIngredienteTxt);
            prefabI.transform.SetParent(requisitosContainer.gameObject.transform, false);
            prefabI.gameObject.name = requisito;

            prefabI.GetComponent<Text>().text = requisito;
            
            string ingredienteTrimmeado = requisito.Trim(new Char[]{' ', '.', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'});

            switch(nombre){
                case "Pegote húmedo":
                prefabI.transform.GetChild(0).gameObject.SetActive(false);
                break;

                case "Pavo asado":
                prefabI.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(ingredienteTrimmeado);              
                if(prefabI.gameObject.name == "0.5 Vegetales ó Frutas"){
                    prefabI.transform.parent.GetChild(2).GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Vegetales");
                    prefabI.transform.parent.GetChild(2).GetChild(1).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Frutas");
                    prefabI.transform.parent.GetChild(2).GetChild(1).gameObject.SetActive(true);   
                }
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
                string restriccionTrimmeado = restriccion.Trim( new Char[] {' ', '.', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'});
                prefabR.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(restriccionTrimmeado);
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

        //Temporal de esta forma: cambiar el tamaño del panel dependiendo la cantidad de restricciones
        // if(restriccionesContainer.transform.childCount == 0){
        //     panelInfo.GetComponent<RectTransform>().offsetMin = new Vector2(15, 645);
        //     restriccionesContainer.transform.parent.gameObject.SetActive(false);
        // }
        
        // if(restriccionesContainer.transform.childCount >= 1 && restriccionesContainer.transform.childCount <= 3){
        //     panelInfo.GetComponent<RectTransform>().offsetMin = new Vector2(15, 190);
        //     restriccionesContainer.transform.parent.gameObject.SetActive(true);
        // } 

        // if(restriccionesContainer.transform.childCount >= 4){
        //     panelInfo.GetComponent<RectTransform>().offsetMin = new Vector2(15, 40);
        //     restriccionesContainer.transform.parent.gameObject.SetActive(true);
        // } 
        
        panelInfo.SetActive(true);

    }
    
    #endregion
}