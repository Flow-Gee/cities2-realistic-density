import React from 'react'
import $Section from '../components/_section'
import $Header from '../components/_header'
import $Description from '../components/_description'
import $IconPanel from '../components/_icon-panel'
import $ToggleGroup from '../components/_toggle-group'

const $Spawnables = ({ react, data, setData, triggerUpdate }) => {

    const workforceOptions = [
        "Off",
        "Low",
        "Medium",
        "High"
    ];

    const updateData = (field, val) => {
        if (field === "SpawnablesCommercial") {
            setData({ ...data, SpawnablesCommercial: val });
        }
        else if (field === "SpawnablesIndustrial") {
            setData({ ...data, SpawnablesIndustrial: val });
        }
        else if (field === "SpawnablesOffice") {
            setData({ ...data, SpawnablesOffice: val });
        }
        else if (field === "SpawnablesExtractor") {
            setData({ ...data, SpawnablesExtractor: val });
        }
        triggerUpdate(field, val);
    }

    return <$Section style={{ width: '100%' }}>
        <$Header label="Spawnables"></$Header>
        <$Description>Adjust the workforce of different zone types.</$Description>
        <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
            <div style={{ flex: 1, width: '50%', paddingRight: '5rem' }}>
                <$IconPanel label="Commercial" icon="Media/Game/Icons/ZoneCommercial.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.SpawnablesCommercial} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("SpawnablesCommercial", val)} />
                    </div>
                </$IconPanel>
                <$IconPanel label="Industrial" icon="Media/Game/Icons/ZoneIndustrial.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.SpawnablesIndustrial} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("SpawnableslIndustrial", val)} />
                    </div>
                </$IconPanel>
            </div>
            <div style={{ flex: 1, width: '50%', paddingLeft: '5rem' }}>
                <$IconPanel label="Offices" icon="Media/Game/Icons/ZoneOffice.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.SpawnablesOffice} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("SpawnablesOffice", val)} />
                    </div>
                </$IconPanel>
                <$IconPanel label="Extractors" icon="Media/Game/Icons/ZoneExtractors.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.SpawnablesExtractor} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("SpawnablesExtractor", val)} />
                    </div>
                </$IconPanel>
            </div>
        </div>
    </$Section>
}

export default $Spawnables