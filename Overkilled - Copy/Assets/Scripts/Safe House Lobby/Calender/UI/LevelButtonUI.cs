using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    [Tooltip("The level this button will set the selected level to")]
    [SerializeField] Level _level;
    [Tooltip("The locked interface to display when this level is locked")]
    [SerializeField] GameObject _lockedOverlay;
    [Tooltip("The UI element to display that this level is currently selected")]
    [SerializeField] GameObject _selectedOverlay;

    Button _button;
    bool _isLocked = true;

    void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(() =>
        {
            if (_isLocked)
                return;

            LevelSelectManager.Instance.SetLevel(_level);
        });

        LevelSelectManager.OnLevelUnlock += LevelSelectManager_OnLevelUnlock;
    }

    void Start()
    {
        LevelSelectManager.Instance.OnLevelSelectChange += LevelSelectManager_OnLevelSelectChange;

        _selectedOverlay.SetActive(false);
    }

    void OnDestroy()
    {
        LevelSelectManager.OnLevelUnlock -= LevelSelectManager_OnLevelUnlock;
        LevelSelectManager.Instance.OnLevelSelectChange -= LevelSelectManager_OnLevelSelectChange;
    }

    void LevelSelectManager_OnLevelUnlock(Level level, LevelSelectManager manager)
    {
        if (level == _level)
        {
            ToggleLock(false);
        }
    }

    void LevelSelectManager_OnLevelSelectChange(Level level, LevelSelectManager manager)
    {
        if (level == Level.None)
        {
            _selectedOverlay.SetActive(false);
            return;
        }

        if (level == _level)
        {
            _selectedOverlay.SetActive(true);
        }
        else
        {
            _selectedOverlay.SetActive(false);
        }
    }

    void ToggleLock(bool state)
    {
        _isLocked = state;
        _lockedOverlay.SetActive(state);
    }
}
