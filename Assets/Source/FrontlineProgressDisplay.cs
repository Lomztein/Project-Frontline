using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FrontlineProgressDisplay : MonoBehaviour
{
    public Slider Slider;
    public Image Foreground;
    public Image Background;

    private Commander _commander;
    private LineSegment _pathToTarget;

    public void Assign(Commander commander)
    {
        _commander = commander;
        _commander.OnNewTarget += OnNewTarget;
        if (_commander.Target)
        {
            OnNewTarget(_commander, _commander.Target);
        }
    }

    private void OnNewTarget(Commander commander, Commander target)
    {
        var path = Navigation.GetPath(Navigation.GetNearestNode(commander.transform.position), Navigation.GetNearestNode(target.transform.position));
        _pathToTarget = LineSegment.CreateFrom(path.Select(x => x.transform.position));

        Background.color = target.TeamInfo.Color;
        Foreground.color = commander.TeamInfo.Color;
    }

    private void Update()
    {
        if (_pathToTarget != null && _commander && _commander.Target)
        {
            float t = _pathToTarget.GetContinuousIndexOfPosition(_pathToTarget.GetNearestPointOnLines(_commander.Frontline.Position));
            Slider.value = Mathf.Lerp(Slider.value, t, 10f * Time.deltaTime);
        }
    }
}
