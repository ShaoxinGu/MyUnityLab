using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
	private Transform m_UIRoot;
	private Transform m_ImgBanner;
	private Image m_ImgBannerImage;
	private Transform m_BtnBuy;
	private Image m_BtnBuyImage;
	private Button m_BtnBuyButton;
	private Transform m_TxtText;
	private Text m_TxtTextText;
	private Transform m_TxtShopName;
	private Text m_TxtShopNameText;
	private Transform m_BtnBuy1;
	private Image m_BtnBuy1Image;
	private Button m_BtnBuy1Button;
	private Transform m_TxtTextA;
	private Text m_TxtTextAText;

	private void Awake()
	{
		InitUI();
		AddListener();
	}

	private void InitUI()
	{
		m_UIRoot = transform;
		m_ImgBanner = transform.Find("ImgBanner");
		m_ImgBannerImage = m_ImgBanner.GetComponent<Image>();
		m_BtnBuy = transform.Find("BtnBuy");
		m_BtnBuyImage = m_BtnBuy.GetComponent<Image>();
		m_BtnBuyButton = m_BtnBuy.GetComponent<Button>();
		m_TxtText = transform.Find("BtnBuy/TxtText");
		m_TxtTextText = m_TxtText.GetComponent<Text>();
		m_TxtShopName = transform.Find("TxtShopName");
		m_TxtShopNameText = m_TxtShopName.GetComponent<Text>();
		m_BtnBuy1 = transform.Find("BtnBuy1");
		m_BtnBuy1Image = m_BtnBuy1.GetComponent<Image>();
		m_BtnBuy1Button = m_BtnBuy1.GetComponent<Button>();
		m_TxtTextA = transform.Find("BtnBuy1/TxtText");
		m_TxtTextAText = m_TxtTextA.GetComponent<Text>();
	}

	private void AddListener()
	{
		m_BtnBuyButton.onClick.AddListener(OnBtnBuyClick);
		m_BtnBuy1Button.onClick.AddListener(OnBtnBuy1Click);
	}

	private void OnBtnBuyClick()
	{

	}

	private void OnBtnBuy1Click()
	{

	}
}