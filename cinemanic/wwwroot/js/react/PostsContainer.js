import React, { useState, useEffect } from 'react';

function PostsContainer() {
    const [posts, setPosts] = useState([]);

    useEffect(() => {
        const fetchPosts = async () => {
            const data = await fetch('/getpostsjson');
            const posts = await data.json();
            setPosts(posts.slice(0, 3));
        }
        fetchPosts();
    }, []);

    useEffect(() => {
        console.log(posts);
    }, [posts]);

    const loadNextPosts = async () => {
        const data = await fetch('/getpostsjson');
        const posts = await data.json();
        setPosts(posts.slice(3, 6));
    };

    return (
        <div className="container">
            <div className="row">
                {posts.map(post => (
                    <div key={post.id} className="col-md-4">
                        <div className="card mb-4 box-shadow">
                            <div className="card-body">
                                <h5 className="card-title">{post.title.rendered}</h5>
                                <p className="card-text">{post.content.rendered}</p>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            <button className="btn btn-primary" onClick={loadNextPosts}>
                Load Next Posts
            </button>
        </div>
    );
}
