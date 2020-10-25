FlexNetworkTransform

	API:
	        /// <summary>
	        /// Dispatched when server receives data from a client while using client authoritative.
	        /// </summary>
		OnClientDataReceived

                /* To reject data on the server you only have to nullify
                 * the data. For example: obj.Data = null; */

                /* You may also modify the data instead.
                 * For example: obj.Data.Position = Vector3.zero; */

                /* Be aware that data may arrive as LocalSpace or WorldSpace
                 * depending on your FNT settings. When modifying data be sure to
                 * convert when necessary. */

                /* You could even implement your own way of snapping the client
                 * authoritative player back after rejecting the data. In my example
                 * I send the current coordinates of the transform back to the client
                 * in which they teleport to these values. */

		See ClientDataRejector in demos for an example.
	