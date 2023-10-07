<script setup lang="ts">
  import router from '@/router';
  import { useRoute } from "vue-router";
  import { watch } from "vue";
  import { ref } from 'vue'
  import type { Ref } from 'vue'
  import type { Bookmark } from "@/models";

  let loading : Ref<boolean> = ref(false);
  let deleting : Ref<boolean> = ref(false);
  let error : Ref<string | null> = ref(null);
  let bookmark : Ref<Bookmark | null> = ref(null);

  const route = useRoute();

  watch(
      () => route.params,
      async () => {
        await fetchData();
      },
      { immediate: true }
  );

  async function fetchData() {
    bookmark.value = null;
    loading.value = true;

    try {
      const response = await fetch('/api/bookmark/' + route.params.id);
      loading.value = false;
      if (response.ok) {
        bookmark.value = await response.json();
      } else {
        error.value = "API responded with " + response.statusText;
      }
    } catch (err : any) {
      loading.value = false;
      error.value = err.toString();
    }
  }

  function cancel() {
    router.push('/');
  }

  async function deleteBookmark() {
    deleting.value = true;

    try {
      const response = await fetch('/api/bookmark/' + route.params.id, {
        method: "DELETE"
      });
      if (response.ok) {
        await router.push('/');
      } else {
        deleting.value = false;
        error.value = "API responded with " + response.statusText;
      }
    } catch (err : any) {
      error.value = err.toString();
    }
  }
</script>

<template>
  <h1>Delete bookmark</h1>

  <div v-if="loading">Loading...</div>
  <div v-if="error">
    <div class="alert alert-danger" role="alert">
      Bookmark not found or couldn't fetch bookmark info.

      <p class="mt-1">Errormessage: {{ error }}</p>
    </div>
  </div>
  <div v-if="bookmark">
    <p>Are you sure you want to delete this bookmark?</p>
    <fieldset disabled>
      <div class="mb-3">
        <label for="url" class="form-label">URL</label>
        <input type="text" class="form-control" id="url" v-model="bookmark.url" >
      </div>
      <div class="mb-3">
        <label for="title" class="form-label">Title</label>
        <input type="text" class="form-control" id="title" v-model="bookmark.title">
      </div>
    </fieldset>

    <button type="button" class="btn btn-danger" @click="deleteBookmark" v-bind:disabled="deleting">
      <span v-if="deleting" class="spinner-border spinner-border-sm" aria-hidden="true"></span>
      <span v-if="deleting" role="status">Deleting...</span>
      <span v-if="!deleting">Delete </span>
    </button>
    <button type="button" class="btn btn-secondary ms-2" @click="cancel">Cancel</button>
  </div>
</template>
