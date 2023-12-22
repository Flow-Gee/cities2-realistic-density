import React from 'react'

const $Info = ({ react, data, setData, triggerUpdate }) => {

    return <$Section style={{ width: '100%' }}>
        <$Header label="Info"></$Header>

        <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
            <div style={{ flex: 1, width: '50%', paddingRight: '5rem' }}>
                <$Description>Adjust the numbers of several city services of your city.</$Description>
            </div>
            <div style={{ flex: 1, width: '50%', paddingLeft: '5rem' }}>
            </div>
        </div>
        <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
        </div>
    </$Section>
}

export default $Info