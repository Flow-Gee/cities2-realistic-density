import React from 'react'
import $Section from '../components/_section'
import $Header from '../components/_header'
import $Description from '../components/_description'
import $IconPanel from '../components/_icon-panel'
import $ToggleGroup from '../components/_toggle-group'

const $CityServices = ({ react, data, setData, triggerUpdate }) => {

    const workforceOptions = [
        "Off",
        "Low",
        "Medium",
        "High"
    ];

    const updateData = (field, val) => {
        if (field === "CityPowerPlant") {
            setData({ ...data, CityPowerPlant: val });
        }
        else if (field === "CityTransportation") {
            setData({ ...data, CityTransportation: val });
        }
        else if (field === "CitySchool") {
            setData({ ...data, CitySchool: val });
        }
        else if (field === "CityHospital") {
            setData({ ...data, CityHospital: val });
        }
        else if (field === "CityPoliceStation") {
            setData({ ...data, CityPoliceStation: val });
        }
        else if (field === "CityFireStation") {
            setData({ ...data, CityFireStation: val });
        }
        triggerUpdate(field, val);
    }

    return <$Section style={{ width: '100%' }}>
        <$Header label="City Services"></$Header>

        <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
            <div style={{ flex: 1, width: '50%', paddingRight: '5rem' }}>
                <$Description>Adjust the numbers of several city services of your city.</$Description>
            </div>
            <div style={{ flex: 1, width: '50%', paddingLeft: '5rem' }}>
            </div>
        </div>
        <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
            <div style={{ flex: 1, width: '50%', paddingRight: '5rem' }}>
                <$IconPanel label="Power Plants" icon="Media/Game/Icons/Electricity.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.CityPowerPlant} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("CityPowerPlant", val)} />
                    </div>
                </$IconPanel>
                <$IconPanel label="Transportation" icon="Media/Game/Icons/Transportation.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.CityTransportation} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("CityTransportation", val)} />
                    </div>
                </$IconPanel>
                <$IconPanel label="Schools" icon="Media/Game/Icons/Education.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.CitySchool} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("CitySchool", val)} />
                    </div>
                </$IconPanel>
            </div>
            <div style={{ flex: 1, width: '50%', paddingLeft: '5rem' }}>
                <$IconPanel label="Hospitals & Clinics" icon="Media/Game/Icons/Healthcare.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.CityHospital} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("CityHospital", val)} />
                    </div>
                </$IconPanel>
                <$IconPanel label="Police Stations" icon="Media/Game/Icons/Police.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.CityPoliceStation} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("CityPoliceStation", val)} />
                    </div>
                </$IconPanel>
                <$IconPanel label="Fire Stations" icon="Media/Game/Icons/FireSafety.svg" fitChild="true">
                    <div style={{ width: '100%', display: 'flex', flexDirection: 'column' }}>
                        <$ToggleGroup react={react} checked={data.CityFireStation} options={workforceOptions} isHorizontal="true" onChecked={(val) => updateData("CityFireStation", val)} />
                    </div>
                </$IconPanel>
            </div>
        </div>
    </$Section>
}

export default $CityServices